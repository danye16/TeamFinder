using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamFinder.Api.Data;
using TeamFinder.Api.Models;

namespace TeamFinder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioJuegosController : ControllerBase
    {
        private readonly TeamFinderDbContext _context;

        public UsuarioJuegosController(TeamFinderDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioJuego>>> GetUsuarioJuegos()
        {
            return await _context.UsuarioJuegos
                .Include(uj => uj.Usuario)
                .Include(uj => uj.Juego)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioJuego>> GetUsuarioJuego(int id)
        {
            var usuarioJuego = await _context.UsuarioJuegos
                .Include(uj => uj.Usuario)
                .Include(uj => uj.Juego)
                .FirstOrDefaultAsync(uj => uj.Id == id);

            if (usuarioJuego == null)
            {
                return NotFound();
            }

            return usuarioJuego;
        }

        [HttpPost]
        public async Task<ActionResult<UsuarioJuego>> PostUsuarioJuego(UsuarioJuego usuarioJuego)
        {
            // Verificar si ya existe la relación
            var existeRelacion = await _context.UsuarioJuegos
                .AnyAsync(uj => uj.UsuarioId == usuarioJuego.UsuarioId && uj.JuegoId == usuarioJuego.JuegoId);

            if (existeRelacion)
            {
                return BadRequest("El usuario ya tiene este juego en su biblioteca.");
            }

            _context.UsuarioJuegos.Add(usuarioJuego);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuarioJuego), new { id = usuarioJuego.Id }, usuarioJuego);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuarioJuego(int id)
        {
            var usuarioJuego = await _context.UsuarioJuegos.FindAsync(id);
            if (usuarioJuego == null)
            {
                return NotFound();
            }

            _context.UsuarioJuegos.Remove(usuarioJuego);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Juego>>> GetJuegosPorUsuario(int usuarioId)
        {
            var juegos = await _context.UsuarioJuegos
                .Where(uj => uj.UsuarioId == usuarioId)
                .Select(uj => uj.Juego)
                .ToListAsync();

            return juegos;
        }

        [HttpGet("juego/{juegoId}")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosPorJuego(int juegoId)
        {
            var usuarios = await _context.UsuarioJuegos
                .Where(uj => uj.JuegoId == juegoId)
                .Select(uj => uj.Usuario)
                .ToListAsync();

            return usuarios;
        }
    }
}