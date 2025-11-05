using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamFinder.Api.Data;
using TeamFinder.Api.Models;

namespace TeamFinder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreferenciasMatchingController : ControllerBase
    {
        private readonly TeamFinderDbContext _context;

        public PreferenciasMatchingController(TeamFinderDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PreferenciaMatching>>> GetPreferenciasMatching()
        {
            return await _context.PreferenciasMatching
                .Include(p => p.Usuario)
                .Include(p => p.Juego)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PreferenciaMatching>> GetPreferenciaMatching(int id)
        {
            var preferencia = await _context.PreferenciasMatching
                .Include(p => p.Usuario)
                .Include(p => p.Juego)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (preferencia == null)
            {
                return NotFound();
            }

            return preferencia;
        }

        [HttpPost]
        public async Task<ActionResult<PreferenciaMatching>> PostPreferenciaMatching(PreferenciaMatching preferencia)
        {
            // Verificar si ya existe una preferencia para este usuario y juego
            var existePreferencia = await _context.PreferenciasMatching
                .AnyAsync(p => p.UsuarioId == preferencia.UsuarioId && p.JuegoId == preferencia.JuegoId);

            if (existePreferencia)
            {
                return BadRequest("El usuario ya tiene preferencias configuradas para este juego.");
            }

            _context.PreferenciasMatching.Add(preferencia);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPreferenciaMatching), new { id = preferencia.Id }, preferencia);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPreferenciaMatching(int id, PreferenciaMatching preferencia)
        {
            if (id != preferencia.Id)
            {
                return BadRequest();
            }

            _context.Entry(preferencia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PreferenciaMatchingExists(id))
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
        public async Task<IActionResult> DeletePreferenciaMatching(int id)
        {
            var preferencia = await _context.PreferenciasMatching.FindAsync(id);
            if (preferencia == null)
            {
                return NotFound();
            }

            _context.PreferenciasMatching.Remove(preferencia);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<PreferenciaMatching>>> GetPreferenciasPorUsuario(int usuarioId)
        {
            var preferencias = await _context.PreferenciasMatching
                .Where(p => p.UsuarioId == usuarioId)
                .Include(p => p.Juego)
                .ToListAsync();

            return preferencias;
        }

        private bool PreferenciaMatchingExists(int id)
        {
            return _context.PreferenciasMatching.Any(e => e.Id == id);
        }
    }
}