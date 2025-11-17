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
    public class JuegosController : ControllerBase
    {
        private readonly TeamFinderDbContext _context;

        public JuegosController(TeamFinderDbContext context)
        {
            _context = context;
        }

        // GET: api/Juegos/MostrarTodos
        [HttpGet("MostrarTodosJuegos")]
        public async Task<ActionResult<IEnumerable<JuegoDto>>> GetJuegos()
        {
            var juegos = await _context.Juegos
                .Select(j => new JuegoDto
                {
                    Id = j.Id,
                    Nombre = j.Nombre,
                    Categoria = j.Categoria,
                    ImagenUrl = j.ImagenUrl,
                    SteamAppId = j.SteamAppId,
                    FechaCreacion = j.FechaCreacion
                })
                .ToListAsync();

            return Ok(juegos);
        }

        // GET: api/Juegos/BuscarPorId/5
        [HttpGet("BuscarPorId/{id}")]
        public async Task<ActionResult<JuegoDto>> GetJuego(int id)
        {
            var juego = await _context.Juegos
                .Where(j => j.Id == id)
                .Select(j => new JuegoDto
                {
                    Id = j.Id,
                    Nombre = j.Nombre,
                    Categoria = j.Categoria,
                    ImagenUrl = j.ImagenUrl,
                    SteamAppId = j.SteamAppId,
                    FechaCreacion = j.FechaCreacion
                })
                .FirstOrDefaultAsync();

            if (juego == null)
            {
                return NotFound();
            }

            return juego;
        }

        // GET: api/Juegos/BuscarPorSteamId/730
        [HttpGet("BuscarPorSteamId/{steamAppId}")]
        public async Task<ActionResult<JuegoDto>> GetJuegoPorSteamId(int steamAppId)
        {
            var juego = await _context.Juegos
                .Where(j => j.SteamAppId == steamAppId)
                .Select(j => new JuegoDto
                {
                    Id = j.Id,
                    Nombre = j.Nombre,
                    Categoria = j.Categoria,
                    ImagenUrl = j.ImagenUrl,
                    SteamAppId = j.SteamAppId,
                    FechaCreacion = j.FechaCreacion
                })
                .FirstOrDefaultAsync();

            if (juego == null)
            {
                return NotFound();
            }

            return juego;
        }

        // POST: api/Juegos/Crear
        [HttpPost("CrearJuego")]
        public async Task<ActionResult<JuegoDto>> PostJuego(JuegoCreacionDto juegoCreacionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar si ya existe un juego con el mismo SteamAppId
            if (juegoCreacionDto.SteamAppId.HasValue)
            {
                var juegoExistente = await _context.Juegos
                    .FirstOrDefaultAsync(j => j.SteamAppId == juegoCreacionDto.SteamAppId);

                if (juegoExistente != null)
                {
                    return Conflict("Ya existe un juego con este SteamAppId.");
                }
            }

            // Verificar si ya existe un juego con el mismo nombre
            var juegoConMismoNombre = await _context.Juegos
                .FirstOrDefaultAsync(j => j.Nombre.ToLower() == juegoCreacionDto.Nombre.ToLower());

            if (juegoConMismoNombre != null)
            {
                return Conflict("Ya existe un juego con este nombre.");
            }

            var juego = new Juego
            {
                Nombre = juegoCreacionDto.Nombre,
                Categoria = juegoCreacionDto.Categoria,
                ImagenUrl = juegoCreacionDto.ImagenUrl,
                SteamAppId = juegoCreacionDto.SteamAppId,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Juegos.Add(juego);
            await _context.SaveChangesAsync();

            var juegoDto = new JuegoDto
            {
                Id = juego.Id,
                Nombre = juego.Nombre,
                Categoria = juego.Categoria,
                ImagenUrl = juego.ImagenUrl,
                SteamAppId = juego.SteamAppId,
                FechaCreacion = juego.FechaCreacion
            };

            return CreatedAtAction(nameof(GetJuego), new { id = juego.Id }, juegoDto);
        }

        // PUT: api/Juegos/Editar/5
        [HttpPut("EditarJuego/{id}")]
        public async Task<IActionResult> PutJuego(int id, JuegoActualizacionDto juegoActualizacionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var juegoExistente = await _context.Juegos.FindAsync(id);
            if (juegoExistente == null)
            {
                return NotFound();
            }

            // Verificar si el nombre ya existe (si se está cambiando)
            if (juegoExistente.Nombre != juegoActualizacionDto.Nombre)
            {
                var juegoConMismoNombre = await _context.Juegos
                    .FirstOrDefaultAsync(j => j.Nombre.ToLower() == juegoActualizacionDto.Nombre.ToLower() && j.Id != id);

                if (juegoConMismoNombre != null)
                {
                    return Conflict("Ya existe otro juego con este nombre.");
                }
            }

            // Actualizar propiedades
            juegoExistente.Nombre = juegoActualizacionDto.Nombre;
            juegoExistente.Categoria = juegoActualizacionDto.Categoria;
            juegoExistente.ImagenUrl = juegoActualizacionDto.ImagenUrl;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JuegoExists(id))
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

        // DELETE: api/Juegos/Eliminar/5
        [HttpDelete("EliminarJuego/{id}")]
        public async Task<IActionResult> DeleteJuego(int id)
        {
            var juego = await _context.Juegos.FindAsync(id);
            if (juego == null)
            {
                return NotFound();
            }

            // Verificar si hay relaciones dependientes antes de eliminar
            var tieneUsuarios = await _context.UsuarioJuegos.AnyAsync(uj => uj.JuegoId == id);
            var tieneEventos = await _context.EventosGaming.AnyAsync(e => e.JuegoId == id);
            var tienePreferencias = await _context.PreferenciasMatching.AnyAsync(p => p.JuegoId == id);
            var tieneMatches = await _context.Matches.AnyAsync(m => m.JuegoId == id);

            if (tieneUsuarios || tieneEventos || tienePreferencias || tieneMatches)
            {
                return BadRequest("No se puede eliminar el juego porque tiene relaciones dependientes. Elimine primero las relaciones.");
            }

            _context.Juegos.Remove(juego);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JuegoExists(int id)
        {
            return _context.Juegos.Any(e => e.Id == id);
        }
    }
}