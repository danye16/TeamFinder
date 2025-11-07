using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TeamFinder.Api.Data;
using TeamFinder.Api.Models;
using TeamFinder.Api.Hubs;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mensaje>>> GetMensajes()
        {
            return await _context.Mensajes
                .Include(m => m.Remitente)
                .Include(m => m.Destinatario)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Mensaje>> GetMensaje(int id)
        {
            var mensaje = await _context.Mensajes
                .Include(m => m.Remitente)
                .Include(m => m.Destinatario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mensaje == null)
            {
                return NotFound();
            }

            return mensaje;
        }

        [HttpPost]
        public async Task<ActionResult<Mensaje>> PostMensaje(Mensaje mensaje)
        {
            _context.Mensajes.Add(mensaje);
            await _context.SaveChangesAsync();

            // Enviar notificación en tiempo real a través de SignalR
            await _hubContext.Clients.User(mensaje.DestinatarioId.ToString())
                .SendAsync("RecibirMensaje", mensaje.RemitenteId, mensaje.Contenido);

            return CreatedAtAction(nameof(GetMensaje), new { id = mensaje.Id }, mensaje);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMensaje(int id, Mensaje mensaje)
        {
            if (id != mensaje.Id)
            {
                return BadRequest();
            }

            _context.Entry(mensaje).State = EntityState.Modified;

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

        [HttpDelete("{id}")]
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

        [HttpGet("conversacion/{usuarioId1}/{usuarioId2}")]
        public async Task<ActionResult<IEnumerable<Mensaje>>> GetConversacion(int usuarioId1, int usuarioId2)
        {
            var mensajes = await _context.Mensajes
                .Where(m => (m.RemitenteId == usuarioId1 && m.DestinatarioId == usuarioId2) ||
                           (m.RemitenteId == usuarioId2 && m.DestinatarioId == usuarioId1))
                .Include(m => m.Remitente)
                .Include(m => m.Destinatario)
                .OrderBy(m => m.FechaEnvio)
                .ToListAsync();

            return mensajes;
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Mensaje>>> GetMensajesPorUsuario(int usuarioId)
        {
            var mensajes = await _context.Mensajes
                .Where(m => m.RemitenteId == usuarioId || m.DestinatarioId == usuarioId)
                .Include(m => m.Remitente)
                .Include(m => m.Destinatario)
                .OrderByDescending(m => m.FechaEnvio)
                .ToListAsync();

            return mensajes;
        }

        [HttpPut("marcar-leidos/{remitenteId}/{destinatarioId}")]
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

            return NoContent();
        }

        private bool MensajeExists(int id)
        {
            return _context.Mensajes.Any(e => e.Id == id);
        }
    }
}