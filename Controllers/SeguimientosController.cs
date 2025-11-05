using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamFinder.Api.Data;
using TeamFinder.Api.Models;

namespace TeamFinder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeguimientosController : ControllerBase
    {
        private readonly TeamFinderDbContext _context;

        public SeguimientosController(TeamFinderDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SeguirUsuario>>> GetSeguimientos()
        {
            return await _context.Seguimientos
                .Include(s => s.Usuario)
                .Include(s => s.Seguido)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SeguirUsuario>> GetSeguimiento(int id)
        {
            var seguimiento = await _context.Seguimientos
                .Include(s => s.Usuario)
                .Include(s => s.Seguido)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (seguimiento == null)
            {
                return NotFound();
            }

            return seguimiento;
        }

        [HttpPost]
        public async Task<ActionResult<SeguirUsuario>> PostSeguimiento(SeguirUsuario seguimiento)
        {
            // Verificar si ya existe el seguimiento
            var existeSeguimiento = await _context.Seguimientos
                .AnyAsync(s => s.UsuarioId == seguimiento.UsuarioId && s.SeguidoId == seguimiento.SeguidoId);

            if (existeSeguimiento)
            {
                return BadRequest("El usuario ya está siguiendo a esta persona.");
            }

            _context.Seguimientos.Add(seguimiento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSeguimiento), new { id = seguimiento.Id }, seguimiento);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeguimiento(int id)
        {
            var seguimiento = await _context.Seguimientos.FindAsync(id);
            if (seguimiento == null)
            {
                return NotFound();
            }

            _context.Seguimientos.Remove(seguimiento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("usuario/{usuarioId}/siguiendo")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosSiguiendo(int usuarioId)
        {
            var usuariosSiguiendo = await _context.Seguimientos
                .Where(s => s.UsuarioId == usuarioId)
                .Select(s => s.Seguido)
                .ToListAsync();

            return usuariosSiguiendo;
        }

        [HttpGet("usuario/{usuarioId}/seguidores")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetSeguidores(int usuarioId)
        {
            var seguidores = await _context.Seguimientos
                .Where(s => s.SeguidoId == usuarioId)
                .Select(s => s.Usuario)
                .ToListAsync();

            return seguidores;
        }
    }
}