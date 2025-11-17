using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TeamFinder.Api.Data;
using TeamFinder.Api.Models;
using TeamFinder.Api.Hubs;
using TeamFinder.Shared.Dtos;
using TeamFinder.Dtos;

namespace TeamFinder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MensajesController : ControllerBase
    {
        private readonly TeamFinderDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public MensajesController(TeamFinderDbContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/Mensajes/MostrarTodos
        [HttpGet("MostrarTodos")]
        public async Task<ActionResult<IEnumerable<MensajeDto>>> GetMensajes()
        {
            var mensajes = await _context.Mensajes
                .Include(m => m.Remitente)
                .Include(m => m.Destinatario)
                .Select(m => new MensajeDto
                {
                    Id = m.Id,
                    RemitenteId = m.RemitenteId,
                    DestinatarioId = m.DestinatarioId,
                    Contenido = m.Contenido,
                    FechaEnvio = m.FechaEnvio,
                    Leido = m.Leido,
                    RemitenteUsername = m.Remitente.Username,
                    DestinatarioUsername = m.Destinatario.Username
                })
                .ToListAsync();

            return Ok(mensajes);
        }

        // GET: api/Mensajes/BuscarPorId/5
        [HttpGet("BuscarPorId/{id}")]
        public async Task<ActionResult<MensajeDto>> GetMensaje(int id)
        {
            var mensaje = await _context.Mensajes
                .Include(m => m.Remitente)
                .Include(m => m.Destinatario)
                .Where(m => m.Id == id)
                .Select(m => new MensajeDto
                {
                    Id = m.Id,
                    RemitenteId = m.RemitenteId,
                    DestinatarioId = m.DestinatarioId,
                    Contenido = m.Contenido,
                    FechaEnvio = m.FechaEnvio,
                    Leido = m.Leido,
                    RemitenteUsername = m.Remitente.Username,
                    DestinatarioUsername = m.Destinatario.Username
                })
                .FirstOrDefaultAsync();

            if (mensaje == null)
            {
                return NotFound();
            }

            return mensaje;
        }

        // POST: api/Mensajes/Enviar
        [HttpPost("EnviarMensaje")]
        public async Task<ActionResult<MensajeDto>> PostMensaje(MensajeCreacionDto mensajeCreacionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar que remitente y destinatario existen
            var remitenteExiste = await _context.Usuarios.AnyAsync(u => u.Id == mensajeCreacionDto.RemitenteId);
            var destinatarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == mensajeCreacionDto.DestinatarioId);

            if (!remitenteExiste || !destinatarioExiste)
            {
                return BadRequest("Remitente o destinatario no válido.");
            }

            var mensaje = new Mensaje
            {
                RemitenteId = mensajeCreacionDto.RemitenteId,
                DestinatarioId = mensajeCreacionDto.DestinatarioId,
                Contenido = mensajeCreacionDto.Contenido,
                FechaEnvio = DateTime.UtcNow,
                Leido = false
            };

            // 1. PRIMERO: Guardar el mensaje en la base de datos
            _context.Mensajes.Add(mensaje);
            await _context.SaveChangesAsync();

            // 2. SEGUNDO: Cargar los datos relacionados para el DTO
            var mensajeConDatos = await _context.Mensajes
                .Include(m => m.Remitente)
                .Include(m => m.Destinatario)
                .Where(m => m.Id == mensaje.Id)
                .Select(m => new MensajeDto
                {
                    Id = m.Id,
                    RemitenteId = m.RemitenteId,
                    DestinatarioId = m.DestinatarioId,
                    Contenido = m.Contenido,
                    FechaEnvio = m.FechaEnvio,
                    Leido = m.Leido,
                    RemitenteUsername = m.Remitente.Username,
                    DestinatarioUsername = m.Destinatario.Username
                })
                .FirstOrDefaultAsync();

            if (mensajeConDatos == null)
            {
                return StatusCode(500, "Error al crear el mensaje.");
            }

            // 3. TERCERO: Enviar notificaciones de SignalR
            var grupo = ObtenerNombreGrupo(mensaje.RemitenteId, mensaje.DestinatarioId);

            // Notificar al grupo de la conversación
            await _hubContext.Clients.Group(grupo)
                .SendAsync("NuevoMensaje", new
                {
                    mensajeConDatos.Id,
                    mensajeConDatos.RemitenteId,
                    mensajeConDatos.RemitenteUsername,
                    mensajeConDatos.DestinatarioId,
                    mensajeConDatos.Contenido,
                    mensajeConDatos.FechaEnvio,
                    mensajeConDatos.Leido
                });

            // También notificar al destinatario específico por si no está en el grupo
            await _hubContext.Clients.User(mensaje.DestinatarioId.ToString())
                .SendAsync("RecibirMensaje", new
                {
                    mensajeConDatos.Id,
                    mensajeConDatos.RemitenteId,
                    mensajeConDatos.RemitenteUsername,
                    mensajeConDatos.Contenido,
                    mensajeConDatos.FechaEnvio
                });

            return CreatedAtAction(nameof(GetMensaje), new { id = mensajeConDatos.Id }, mensajeConDatos);
        }

        // PUT: api/Mensajes/Editar/5
        [HttpPut("EditarMensaje/{id}")]
        public async Task<IActionResult> PutMensaje(int id, MensajeActualizacionDto mensajeActualizacionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mensajeExistente = await _context.Mensajes.FindAsync(id);
            if (mensajeExistente == null)
            {
                return NotFound();
            }

            // Solo permitir actualizar el contenido y el estado de leído
            mensajeExistente.Contenido = mensajeActualizacionDto.Contenido;
            mensajeExistente.Leido = mensajeActualizacionDto.Leido;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MensajeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Mensajes/Eliminar/5
        [HttpDelete("EliminarMensaje/{id}")]
        public async Task<IActionResult> DeleteMensaje(int id)
        {
            var mensaje = await _context.Mensajes.FindAsync(id);
            if (mensaje == null)
            {
                return NotFound();
            }

            _context.Mensajes.Remove(mensaje);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Mensajes/Conversacion/1/2
        [HttpGet("Conversacion/{usuarioId1}/{usuarioId2}")]
        public async Task<ActionResult<IEnumerable<MensajeDto>>> GetConversacion(int usuarioId1, int usuarioId2)
        {
            var mensajes = await _context.Mensajes
                .Where(m => (m.RemitenteId == usuarioId1 && m.DestinatarioId == usuarioId2) ||
                           (m.RemitenteId == usuarioId2 && m.DestinatarioId == usuarioId1))
                .Include(m => m.Remitente)
                .Include(m => m.Destinatario)
                .OrderBy(m => m.FechaEnvio)
                .Select(m => new MensajeDto
                {
                    Id = m.Id,
                    RemitenteId = m.RemitenteId,
                    DestinatarioId = m.DestinatarioId,
                    Contenido = m.Contenido,
                    FechaEnvio = m.FechaEnvio,
                    Leido = m.Leido,
                    RemitenteUsername = m.Remitente.Username,
                    DestinatarioUsername = m.Destinatario.Username
                })
                .ToListAsync();

            return Ok(mensajes);
        }

        // GET: api/Mensajes/Usuario/1
        [HttpGet("Usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<MensajeDto>>> GetMensajesPorUsuario(int usuarioId)
        {
            var mensajes = await _context.Mensajes
                .Where(m => m.RemitenteId == usuarioId || m.DestinatarioId == usuarioId)
                .Include(m => m.Remitente)
                .Include(m => m.Destinatario)
                .OrderByDescending(m => m.FechaEnvio)
                .Select(m => new MensajeDto
                {
                    Id = m.Id,
                    RemitenteId = m.RemitenteId,
                    DestinatarioId = m.DestinatarioId,
                    Contenido = m.Contenido,
                    FechaEnvio = m.FechaEnvio,
                    Leido = m.Leido,
                    RemitenteUsername = m.Remitente.Username,
                    DestinatarioUsername = m.Destinatario.Username
                })
                .ToListAsync();

            return Ok(mensajes);
        }

        // PUT: api/Mensajes/MarcarLeidos/1/2
        [HttpPut("MarcarLeidos/{remitenteId}/{destinatarioId}")]
        public async Task<IActionResult> MarcarMensajesComoLeidos(int remitenteId, int destinatarioId)
        {
            var mensajesNoLeidos = await _context.Mensajes
                .Where(m => m.RemitenteId == remitenteId && m.DestinatarioId == destinatarioId && !m.Leido)
                .ToListAsync();

            foreach (var mensaje in mensajesNoLeidos)
            {
                mensaje.Leido = true;
            }

            await _context.SaveChangesAsync();

            var grupo = ObtenerNombreGrupo(remitenteId, destinatarioId);
            await _hubContext.Clients.Group(grupo)
                .SendAsync("MensajesLeidos", new { remitenteId, destinatarioId });

            return NoContent();
        }


        private string ObtenerNombreGrupo(int usuarioId1, int usuarioId2)
        {
            return usuarioId1 < usuarioId2 ?
                $"Conversacion_{usuarioId1}_{usuarioId2}" :
                $"Conversacion_{usuarioId2}_{usuarioId1}";
        }

        // GET: api/Mensajes/NoLeidos/1
        [HttpGet("NoLeidos/{usuarioId}")]
        public async Task<ActionResult<int>> GetMensajesNoLeidos(int usuarioId)
        {
            var cantidadNoLeidos = await _context.Mensajes
                .Where(m => m.DestinatarioId == usuarioId && !m.Leido)
                .CountAsync();

            return Ok(cantidadNoLeidos);
        }

        private bool MensajeExists(int id)
        {
            return _context.Mensajes.Any(e => e.Id == id);
        }
    }
}