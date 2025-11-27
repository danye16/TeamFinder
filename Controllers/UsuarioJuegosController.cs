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

        [HttpGet("MostrarUsuariosJuegos")]
        public async Task<ActionResult<IEnumerable<UsuarioJuegoDto>>> GetUsuarioJuegos()
        {
            var usuarioJuegos = await _context.UsuarioJuegos
                .Include(uj => uj.Usuario)
                .Include(uj => uj.Juego)
                .Select(uj => new UsuarioJuegoDto
                {
                    Id = uj.Id,
                    UsuarioId = uj.UsuarioId,
                    JuegoId = uj.JuegoId,
                    FechaAgregado = uj.FechaAgregado,
                    UsuarioUsername = uj.Usuario.Username,
                    UsuarioPais = uj.Usuario.Pais,
                    UsuarioEdad = uj.Usuario.Edad,
                    UsuarioEstiloJuego = uj.Usuario.EstiloJuego,
                    JuegoNombre = uj.Juego.Nombre,
                    JuegoCategoria = uj.Juego.Categoria,
                    JuegoImagenUrl = uj.Juego.ImagenUrl
                })
                .ToListAsync();

            return Ok(usuarioJuegos);
        }

        [HttpGet("BuscarUsuarioJuegoEspecifico/{id}")]
        public async Task<ActionResult<UsuarioJuegoDto>> GetUsuarioJuego(int id)
        {
            var usuarioJuego = await _context.UsuarioJuegos
                .Include(uj => uj.Usuario)
                .Include(uj => uj.Juego)
                .Where(uj => uj.Id == id)
                .Select(uj => new UsuarioJuegoDto
                {
                    Id = uj.Id,
                    UsuarioId = uj.UsuarioId,
                    JuegoId = uj.JuegoId,
                    FechaAgregado = uj.FechaAgregado,
                    UsuarioUsername = uj.Usuario.Username,
                    UsuarioPais = uj.Usuario.Pais,
                    UsuarioEdad = uj.Usuario.Edad,
                    UsuarioEstiloJuego = uj.Usuario.EstiloJuego,
                    JuegoNombre = uj.Juego.Nombre,
                    JuegoCategoria = uj.Juego.Categoria,
                    JuegoImagenUrl = uj.Juego.ImagenUrl
                })
                .FirstOrDefaultAsync();

            if (usuarioJuego == null)
            {
                return NotFound();
            }

            return usuarioJuego;
        }

        [HttpGet("BuscarJugadoresPorSteamId/{steamAppId}")]
        public async Task<ActionResult<IEnumerable<UsuarioJuegoDto>>> GetUsuariosPorSteamAppId(int steamAppId)
        {
            // 1. Buscamos cuál es el ID interno (ej: 5) usando el ID de Steam (ej: 730)
            var juegoInterno = await _context.Juegos
                .FirstOrDefaultAsync(j => j.SteamAppId == steamAppId);

            if (juegoInterno == null)
            {
                // Si nadie ha registrado el juego, devolvemos lista vacía en vez de error
                return Ok(new List<UsuarioJuegoDto>());
            }

            // 2. Buscamos los usuarios usando el ID interno que acabamos de encontrar
            var usuariosJuegos = await _context.UsuarioJuegos
                .Include(uj => uj.Usuario)
                .Include(uj => uj.Juego) // Incluimos juego para llenar el DTO completo
                .Where(uj => uj.JuegoId == juegoInterno.Id)
                .Select(uj => new UsuarioJuegoDto
                {
                    Id = uj.Id,
                    UsuarioId = uj.UsuarioId,
                    JuegoId = uj.JuegoId,
                    FechaAgregado = uj.FechaAgregado,
                    UsuarioUsername = uj.Usuario.Username,
                    UsuarioPais = uj.Usuario.Pais,
                    UsuarioEdad = uj.Usuario.Edad,
                    UsuarioEstiloJuego = uj.Usuario.EstiloJuego,
                    JuegoNombre = uj.Juego.Nombre,
                    JuegoCategoria = uj.Juego.Categoria,
                    JuegoImagenUrl = uj.Juego.ImagenUrl
                })
                .ToListAsync();

            return Ok(usuariosJuegos);
        }


        [HttpPost("CrearUsuarioJuego")]
        public async Task<ActionResult<UsuarioJuegoDto>> PostUsuarioJuego(UsuarioJuegoCreacionDto usuarioJuegoCreacionDto)
        {
            // Verificar si ya existe la relación
            var existeRelacion = await _context.UsuarioJuegos
                .AnyAsync(uj => uj.UsuarioId == usuarioJuegoCreacionDto.UsuarioId && uj.JuegoId == usuarioJuegoCreacionDto.JuegoId);

            if (existeRelacion)
            {
                return BadRequest("El usuario ya tiene este juego en su biblioteca.");
            }

            // Verificar que el usuario existe
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == usuarioJuegoCreacionDto.UsuarioId);
            if (!usuarioExiste)
            {
                return BadRequest("El usuario no existe.");
            }

            // Verificar que el juego existe
            var juegoExiste = await _context.Juegos.AnyAsync(j => j.Id == usuarioJuegoCreacionDto.JuegoId);
            if (!juegoExiste)
            {
                return BadRequest("El juego no existe.");
            }

            // Crear la nueva relación
            var usuarioJuego = new UsuarioJuego
            {
                UsuarioId = usuarioJuegoCreacionDto.UsuarioId,
                JuegoId = usuarioJuegoCreacionDto.JuegoId,
                FechaAgregado = DateTime.UtcNow
            };

            _context.UsuarioJuegos.Add(usuarioJuego);
            await _context.SaveChangesAsync();

            // Cargar los datos relacionados para retornar el DTO completo
            var usuarioJuegoConDatos = await _context.UsuarioJuegos
                .Include(uj => uj.Usuario)
                .Include(uj => uj.Juego)
                .Where(uj => uj.Id == usuarioJuego.Id)
                .Select(uj => new UsuarioJuegoDto
                {
                    Id = uj.Id,
                    UsuarioId = uj.UsuarioId,
                    JuegoId = uj.JuegoId,
                    FechaAgregado = uj.FechaAgregado,
                    UsuarioUsername = uj.Usuario.Username,
                    UsuarioPais = uj.Usuario.Pais,
                    UsuarioEdad = uj.Usuario.Edad,
                    UsuarioEstiloJuego = uj.Usuario.EstiloJuego,
                    JuegoNombre = uj.Juego.Nombre,
                    JuegoCategoria = uj.Juego.Categoria,
                    JuegoImagenUrl = uj.Juego.ImagenUrl
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetUsuarioJuego), new { id = usuarioJuegoConDatos.Id }, usuarioJuegoConDatos);
        }

        [HttpPost("AgregarJuegoDesdeSteam")]
        public async Task<ActionResult<UsuarioJuegoDto>> AgregarJuegoDesdeSteam(UsuarioJuegoSteamCreacionDto dto)
        {
            // Verificar que el usuario existe
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == dto.UsuarioId);
            if (!usuarioExiste)
            {
                return BadRequest("El usuario no existe.");
            }

            // Buscar o crear el juego
            var juego = await ObtenerOCrearJuego(dto.SteamAppId, dto.Nombre, dto.Categoria, dto.ImagenUrl);

            // Verificar si ya existe la relación Usuario-Juego
            var existeRelacion = await _context.UsuarioJuegos
                .AnyAsync(uj => uj.UsuarioId == dto.UsuarioId && uj.JuegoId == juego.Id);

            if (existeRelacion)
            {
                return BadRequest("El usuario ya tiene este juego en su biblioteca.");
            }

            // Crear la relación UsuarioJuego
            var usuarioJuego = new UsuarioJuego
            {
                UsuarioId = dto.UsuarioId,
                JuegoId = juego.Id,
                FechaAgregado = DateTime.UtcNow
            };

            _context.UsuarioJuegos.Add(usuarioJuego);
            await _context.SaveChangesAsync();

            // Retornar el resultado con los datos completos
            var resultado = await _context.UsuarioJuegos
                .Include(uj => uj.Usuario)
                .Include(uj => uj.Juego)
                .Where(uj => uj.Id == usuarioJuego.Id)
                .Select(uj => new UsuarioJuegoDto
                {
                    Id = uj.Id,
                    UsuarioId = uj.UsuarioId,
                    JuegoId = uj.JuegoId,
                    FechaAgregado = uj.FechaAgregado,
                    UsuarioUsername = uj.Usuario.Username,
                    UsuarioPais = uj.Usuario.Pais,
                    UsuarioEdad = uj.Usuario.Edad,
                    UsuarioEstiloJuego = uj.Usuario.EstiloJuego,
                    JuegoNombre = uj.Juego.Nombre,
                    JuegoCategoria = uj.Juego.Categoria,
                    JuegoImagenUrl = uj.Juego.ImagenUrl
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetUsuarioJuego), new { id = resultado.Id }, resultado);
        }

        private async Task<Juego> ObtenerOCrearJuego(int steamAppId, string nombre, string categoria, string imagenUrl)
        {
            // Buscar por SteamAppId (lo más confiable)
            var juegoExistente = await _context.Juegos
                .FirstOrDefaultAsync(j => j.SteamAppId == steamAppId);

            if (juegoExistente != null)
            {
                return juegoExistente;
            }

            // Si no existe, crear nuevo juego
            var nuevoJuego = new Juego
            {
                Nombre = nombre,
                Categoria = categoria,
                ImagenUrl = imagenUrl,
                SteamAppId = steamAppId,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Juegos.Add(nuevoJuego);
            await _context.SaveChangesAsync();

            return nuevoJuego;
        }

        [HttpDelete("EliminarUsuarioJuego/{id}")]
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

        [HttpGet("BuscarConUsuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Juego>>> GetJuegosPorUsuario(int usuarioId)
        {
            var juegos = await _context.UsuarioJuegos
                .Where(uj => uj.UsuarioId == usuarioId)
                .Select(uj => uj.Juego)
                .ToListAsync();

            return juegos;
        }

        [HttpGet("BuscarConJuego/{juegoId}")]
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