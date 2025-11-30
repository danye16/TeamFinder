using Microsoft.AspNetCore.Mvc;
using TeamFinder.Api.Services;
using TeamFinder.Api.Models;
using TeamFinder.Shared.Dtos;
using TeamFinder.Dtos;

namespace TeamFinder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchingService _matchingService;

        public MatchesController(IMatchingService matchingService)
        {
            _matchingService = matchingService;
        }

        // GET: api/Matches/BuscarMatches/1/1
        [HttpGet("BuscarMatches/{usuarioId}/{juegoId}")]
        public async Task<ActionResult<IEnumerable<MatchResultDto>>> BuscarMatches(int usuarioId, int juegoId)
        {
            var matches = await _matchingService.EncontrarMatchesDetalladosAsync(usuarioId, juegoId);
            
            var matchesDto = matches.Select(m => new MatchResultDto
            {
                Usuario = new UsuarioDto
                {
                    Id = m.Usuario.Id,
                    Username = m.Usuario.Username,
                    Pais = m.Usuario.Pais,
                    Edad = m.Usuario.Edad,
                    EstiloJuego = m.Usuario.EstiloJuego
                },
                PorcentajeMatch = m.PorcentajeMatch,
                RazonesMatch = m.RazonesMatch,
                NivelHabilidad = m.PreferenciaMatching.NivelHabilidad,
                EstiloJuego = m.PreferenciaMatching.EstiloJuego,
                RolPreferido = m.PreferenciaMatching.RolPreferido
            });

            return Ok(matchesDto);
        }

        // POST: api/Matches/CalcularMatch
        [HttpPost("CalcularMatch")]
        public async Task<ActionResult<double>> CalcularMatch([FromBody] MatchRequest request)
        {
            var porcentaje = await _matchingService.CalcularPorcentajeMatchAsync(request.UsuarioId1, request.UsuarioId2, request.JuegoId);
            return Ok(porcentaje);
        }

        // POST: api/Matches/CrearMatch
        [HttpPost("CrearMatch")]
        public async Task<ActionResult<MatchDto>> CrearMatch([FromBody] MatchCreacionDto matchCreacionDto)
        {
            // 1. Llamamos al servicio pasando el ID de quien inicia (asumiendo que es Usuario1Id)
            // El servicio nos devuelve la ENTIDAD 'Match' (con los datos de la BD)
            var match = await _matchingService.CrearMatchAsync(
                matchCreacionDto.Usuario1Id,
                matchCreacionDto.Usuario2Id,
                matchCreacionDto.JuegoId,
                matchCreacionDto.Usuario1Id // <--- ID del iniciador (quien dio click)
            );

            if (match == null)
            {
                return BadRequest("No se pudo crear el match. Verifica que los usuarios existan.");
            }

            // 2. Mapeamos la Entidad 'match' hacia tu 'MatchDto'
            var matchDto = new MatchDto
            {
                Id = match.Id,
                Usuario1Id = match.Usuario1Id,
                Usuario2Id = match.Usuario2Id,
                JuegoId = match.JuegoId,
                FechaMatch = match.FechaMatch,

                // Estas propiedades ahora vendrán con 'true' para el iniciador
                // gracias al cambio que hicimos en el servicio.
                AceptadoPorUsuario1 = match.AceptadoPorUsuario1,
                AceptadoPorUsuario2 = match.AceptadoPorUsuario2,
                MatchConfirmado = match.MatchConfirmado
            };

            // 3. Retornamos el DTO
            return Ok(matchDto);
        }

        // PUT: api/Matches/AceptarMatch/1/1
        [HttpPut("AceptarMatch/{matchId}/{usuarioId}")]
        public async Task<IActionResult> AceptarMatch(int matchId, int usuarioId)
        {
            var resultado = await _matchingService.AceptarMatchAsync(matchId, usuarioId);

            if (!resultado)
            {
                return BadRequest("No se pudo aceptar el match. Verifica los datos e intenta de nuevo.");
            }

            return Ok(new { message = "Match aceptado exitosamente" });
        }

        // GET: api/Matches/Pendientes/1
        [HttpGet("Pendientes/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<MatchDetalleDto>>> ObtenerMatchesPendientes(int usuarioId)
        {
            var matches = await _matchingService.ObtenerMatchesPendientesAsync(usuarioId);
            // Mapeamos a MatchDetalleDto para tener los nombres y avatares
            var matchesDto = matches.Select(m => new MatchDetalleDto
            {
                Id = m.Id,
                Usuario1 = new UsuarioDto { Id = m.Usuario1.Id, Username = m.Usuario1.Username, AvatarUrl = m.Usuario1.AvatarUrl }, // Asegúrate de incluir AvatarUrl si lo tienes en UsuarioDto
                Usuario2 = new UsuarioDto { Id = m.Usuario2.Id, Username = m.Usuario2.Username, AvatarUrl = m.Usuario2.AvatarUrl },
                Juego = new JuegoDto { Id = m.Juego.Id, Nombre = m.Juego.Nombre, ImagenUrl = m.Juego.ImagenUrl },
                MatchConfirmado = m.MatchConfirmado,
                AceptadoPorUsuario1 = m.AceptadoPorUsuario1,
                AceptadoPorUsuario2 = m.AceptadoPorUsuario2
            });
            return Ok(matchesDto);
        }
        // GET: api/Matches/Confirmados/1
        [HttpGet("Confirmados/{usuarioId}")]
        // 1. Cambia 'MatchDto' por 'MatchDetalleDto' en el tipo de retorno
        public async Task<ActionResult<IEnumerable<MatchDetalleDto>>> ObtenerMatchesConfirmados(int usuarioId)
        {
            var matches = await _matchingService.ObtenerMatchesConfirmadosAsync(usuarioId);

            // 2. Cambia 'new MatchDto' por 'new MatchDetalleDto' aquí también
            var matchesDto = matches.Select(m => new MatchDetalleDto
            {
                Id = m.Id,

                // Ahora sí funcionará porque MatchDetalleDto espera objetos completos
                Usuario1 = new UsuarioDto
                {
                    Id = m.Usuario1.Id,
                    Username = m.Usuario1.Username,
                    AvatarUrl = m.Usuario1.AvatarUrl // Asegúrate que tu Usuario tenga esta propiedad
                },

                Usuario2 = new UsuarioDto
                {
                    Id = m.Usuario2.Id,
                    Username = m.Usuario2.Username,
                    AvatarUrl = m.Usuario2.AvatarUrl
                },

                Juego = new JuegoDto
                {
                    Id = m.Juego.Id,
                    Nombre = m.Juego.Nombre,
                    ImagenUrl = m.Juego.ImagenUrl
                },

                MatchConfirmado = m.MatchConfirmado,
                AceptadoPorUsuario1 = m.AceptadoPorUsuario1,
                AceptadoPorUsuario2 = m.AceptadoPorUsuario2
            });

            return Ok(matchesDto);
        }

        // DELETE: api/Matches/RechazarMatch/1/1
        [HttpDelete("RechazarMatch/{matchId}/{usuarioId}")]
        public async Task<IActionResult> RechazarMatch(int matchId, int usuarioId)
        {
            var resultado = await _matchingService.RechazarMatchAsync(matchId, usuarioId);

            if (!resultado)
            {
                return BadRequest("No se pudo rechazar el match. Verifica los datos e intenta de nuevo.");
            }

            return Ok(new { message = "Match rechazado exitosamente" });
        }

        // GET: api/Matches/Detalle/1
        [HttpGet("Detalle/{matchId}")]
        public async Task<ActionResult<MatchDetalleDto>> ObtenerMatchDetalle(int matchId)
        {
            var match = await _matchingService.ObtenerMatchDetalleAsync(matchId);

            if (match == null)
            {
                return NotFound();
            }

            var matchDetalleDto = new MatchDetalleDto
            {
                Id = match.Id,
                Usuario1 = new UsuarioDto
                {
                    Id = match.Usuario1.Id,
                    Username = match.Usuario1.Username,
                    Pais = match.Usuario1.Pais,
                    Edad = match.Usuario1.Edad,
                    EstiloJuego = match.Usuario1.EstiloJuego
                },
                Usuario2 = new UsuarioDto
                {
                    Id = match.Usuario2.Id,
                    Username = match.Usuario2.Username,
                    Pais = match.Usuario2.Pais,
                    Edad = match.Usuario2.Edad,
                    EstiloJuego = match.Usuario2.EstiloJuego
                },
                Juego = new JuegoDto
                {
                    Id = match.Juego.Id,
                    Nombre = match.Juego.Nombre,
                    Categoria = match.Juego.Categoria,
                    ImagenUrl = match.Juego.ImagenUrl
                },
                FechaMatch = match.FechaMatch,  
                AceptadoPorUsuario1 = match.AceptadoPorUsuario1,  
                AceptadoPorUsuario2 = match.AceptadoPorUsuario2,  
                MatchConfirmado = match.MatchConfirmado
            };

            return Ok(matchDetalleDto);
        }
    }
}