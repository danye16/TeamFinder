using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamFinder.Api.Data;
using TeamFinder.Api.Models;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Juego>>> GetJuegos()
        {
            return await _context.Juegos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Juego>> GetJuego(int id)
        {
            var juego = await _context.Juegos.FindAsync(id);

            if (juego == null)
            {
                return NotFound();
            }

            return juego;
        }

        [HttpPost]
        public async Task<ActionResult<Juego>> PostJuego(Juego juego)
        {
            _context.Juegos.Add(juego);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetJuego), new { id = juego.Id }, juego);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutJuego(int id, Juego juego)
        {
            if (id != juego.Id)
            {
                return BadRequest();
            }

            _context.Entry(juego).State = EntityState.Modified;

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJuego(int id)
        {
            var juego = await _context.Juegos.FindAsync(id);
            if (juego == null)
            {
                return NotFound();
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