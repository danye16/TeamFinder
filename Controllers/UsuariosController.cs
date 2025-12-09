using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamFinder.Api.Data;
using TeamFinder.Api.Models;
using TeamFinder.Dtos;
using TeamFinder.Shared.Dtos;

namespace TeamFinder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly TeamFinderDbContext _context;

        public class SteamLoginDto
        {
            public string? SteamId { get; set; }
        }
        public UsuariosController(TeamFinderDbContext context)
        {
            _context = context;
        }

        [HttpPost("LoginSteam")]
        public async Task<ActionResult<Usuario>> LoginSteam([FromBody] SteamLoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.SteamId))
            {
                return BadRequest("El SteamId es obligatorio.");
            }

            // Buscamos un usuario que tenga ese SteamId
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.SteamId == loginDto.SteamId);

            // Si no existe, devolvemos 404 para que el Frontend sepa que debe ir a Registrarse
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado. Por favor regístrate primero." });
            }

            // Si existe, lo devolvemos (el Frontend lo usará para guardar el token/sesión)
            return Ok(usuario);
        }

        // Endpoint GET para Login con Steam (NUEVO)
        [HttpGet("LoginSteam")]
        public async Task<ActionResult<Usuario>> GetLoginSteam([FromQuery] string steamId)
        {
            if (string.IsNullOrEmpty(steamId))
            {
                return BadRequest("El SteamId es obligatorio.");
            }

            // Buscamos un usuario que tenga ese SteamId
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.SteamId == steamId);

            // Si no existe, devolvemos 404 para que el Frontend sepa que debe ir a Registrarse
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado. Por favor regístrate primero." });
            }

            // Si existe, lo devolvemos (el Frontend lo usará para guardar el token/sesión)
            return Ok(usuario);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<Usuario>> Login([FromBody] LoginDto loginDto)
        {
            // 1. Buscamos al usuario por nombre
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (usuario == null)
            {
                return Unauthorized("Usuario no encontrado.");
            }

            // 2. Generamos el hash de la contraseña que nos enviaron 
            // (Usando TU misma lógica de registro)
            var hashIntento = PasswordHasher.HashPassword(loginDto.Password);

            // 3. Comparamos los hashes
            if (usuario.Contraseña != hashIntento)
            {
                return Unauthorized("Contraseña mal.");
            }

            // 4. Si todo coincide, devolvemos el usuario
            return Ok(new
            {
                usuario.Id,
                usuario.Username,
                usuario.SteamId,
                usuario.Pais,
                usuario.Edad,
                usuario.EstiloJuego,
                usuario.FechaCreacion,
                usuario.AvatarUrl
                // NO devolvemos la contraseña
            });
        }






        [HttpGet("MostrarUsuarios")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            // El [JsonIgnore] en el modelo se encargará de ocultar la contraseña
            return await _context.Usuarios.ToListAsync();
        }

        [HttpGet("BuscarUsuarioEspecifico/{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            // La contraseña no se incluirá en la respuesta gracias a [JsonIgnore]
            return usuario;
        }

        [HttpPost("CrearUsuario")]
        public async Task<ActionResult<Usuario>> PostUsuario(UsuarioCreacionDto usuarioCreacionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Usuarios.AnyAsync(u => u.Username == usuarioCreacionDto.Username))
            {
                return BadRequest("El nombre de usuario ya existe");
            }

            var usuario = new Usuario
            {
                Username = usuarioCreacionDto.Username,
                Contraseña = PasswordHasher.HashPassword(usuarioCreacionDto.Contraseña), // Contraseña hasheada
                SteamId = usuarioCreacionDto.SteamId,
                Pais = usuarioCreacionDto.Pais,
                Edad = usuarioCreacionDto.Edad,
                EstiloJuego = usuarioCreacionDto.EstiloJuego,
                AvatarUrl = usuarioCreacionDto.AvatarUrl,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, new
            {
                usuario.Id,
                usuario.Username,
                usuario.SteamId,
                usuario.Pais,
                usuario.Edad,
                usuario.EstiloJuego,
                usuario.FechaCreacion,
                usuario.AvatarUrl
            });
        }

        [HttpPut("EditarUsuario/{id}")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioActualizacionDto usuarioActualizacionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Buscar el usuario existente
            var usuarioExistente = await _context.Usuarios.FindAsync(id);
            if (usuarioExistente == null)
            {
                return NotFound();
            }

            // Verificar si el nuevo username ya existe (si es diferente al actual)
            if (usuarioExistente.Username != usuarioActualizacionDto.Username &&
                await _context.Usuarios.AnyAsync(u => u.Username == usuarioActualizacionDto.Username))
            {
                return BadRequest("El nombre de usuario ya existe");
            }

            // Actualizar solo los campos permitidos
            usuarioExistente.Username = usuarioActualizacionDto.Username;
            usuarioExistente.SteamId = usuarioActualizacionDto.SteamId;
            usuarioExistente.Pais = usuarioActualizacionDto.Pais;
            usuarioExistente.Edad = usuarioActualizacionDto.Edad;
            usuarioExistente.EstiloJuego = usuarioActualizacionDto.EstiloJuego;

            // Actualizar contraseña si se proporciona una nueva
            if (!string.IsNullOrEmpty(usuarioActualizacionDto.NuevaContraseña))
            {
                usuarioExistente.Contraseña = PasswordHasher.HashPassword(usuarioActualizacionDto.NuevaContraseña);
            }

            if (!string.IsNullOrEmpty(usuarioActualizacionDto.AvatarUrl))
            {
                usuarioExistente.AvatarUrl = usuarioActualizacionDto.AvatarUrl;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Devolver el usuario actualizado (sin contraseña)
            return Ok(new
            {
                usuarioExistente.Id,
                usuarioExistente.Username,
                usuarioExistente.SteamId,
                usuarioExistente.Pais,
                usuarioExistente.Edad,
                usuarioExistente.EstiloJuego,
                usuarioExistente.FechaCreacion,
                usuarioExistente.AvatarUrl
            });
        }

        [HttpDelete("EliminarUsuario/{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Eliminar dependencias en orden para evitar conflictos de FK
            await EliminarDependenciasUsuario(id);

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task EliminarDependenciasUsuario(int usuarioId)
        {
            // 1. Eliminar mensajes (tanto enviados como recibidos)
            var mensajes = await _context.Mensajes
                .Where(m => m.RemitenteId == usuarioId || m.DestinatarioId == usuarioId)
                .ToListAsync();
            _context.Mensajes.RemoveRange(mensajes);

            // 2. Eliminar evaluaciones (realizadas y recibidas)
            var evaluaciones = await _context.Reputaciones
                .Where(r => r.UsuarioId == usuarioId || r.EvaluadorId == usuarioId)
                .ToListAsync();
            _context.Reputaciones.RemoveRange(evaluaciones);

            // 3. Eliminar relaciones de seguimiento (CORREGIDO: Seguimientos en lugar de SeguirUsuarios)
            var seguimientos = await _context.Seguimientos
                .Where(s => s.UsuarioId == usuarioId || s.SeguidoId == usuarioId)
                .ToListAsync();
            _context.Seguimientos.RemoveRange(seguimientos);

            // 4. Eliminar participación en eventos
            var eventosParticipando = await _context.EventoParticipantes
                .Where(ep => ep.UsuarioId == usuarioId)
                .ToListAsync();
            _context.EventoParticipantes.RemoveRange(eventosParticipando);

            // 5. Manejar eventos organizados
            var eventosOrganizados = await _context.EventosGaming
                .Where(e => e.OrganizadorId == usuarioId)
                .ToListAsync();

            foreach (var evento in eventosOrganizados)
            {
                // Eliminar participantes del evento
                var participantesEvento = await _context.EventoParticipantes
                    .Where(ep => ep.EventoId == evento.Id)
                    .ToListAsync();
                _context.EventoParticipantes.RemoveRange(participantesEvento);

                // Eliminar el evento
                _context.EventosGaming.Remove(evento);
            }

            // 6. Eliminar juegos del usuario
            var usuarioJuegos = await _context.UsuarioJuegos
                .Where(uj => uj.UsuarioId == usuarioId)
                .ToListAsync();
            _context.UsuarioJuegos.RemoveRange(usuarioJuegos);

            // 7. Eliminar insignias del usuario
            var usuarioInsignias = await _context.UsuarioInsignias
                .Where(ui => ui.UsuarioId == usuarioId)
                .ToListAsync();
            _context.UsuarioInsignias.RemoveRange(usuarioInsignias);

            // 8. Eliminar preferencias de matching del usuario
            var preferenciasMatching = await _context.PreferenciasMatching
                .Where(p => p.UsuarioId == usuarioId)
                .ToListAsync();
            _context.PreferenciasMatching.RemoveRange(preferenciasMatching);

            // 9. Eliminar matches del usuario
            var matches = await _context.Matches
                .Where(m => m.Usuario1Id == usuarioId || m.Usuario2Id == usuarioId)
                .ToListAsync();
            _context.Matches.RemoveRange(matches);

            // Guardar cambios de todas las dependencias
            await _context.SaveChangesAsync();
        }



        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}