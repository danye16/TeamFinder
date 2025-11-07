using Microsoft.AspNetCore.Mvc;
using TeamFinder.Api.Services;
using TeamFinder.Api.Models;

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

        [HttpGet("encontrar/{usuarioId}/{juegoId}")]
        public async Task<ActionResult<IEnumerable<Usuario>>> EncontrarMatches(int usuarioId, int juegoId)
        {
            var matches = await _matchingService.EncontrarMatchesAsync(usuarioId, juegoId);
            return Ok(matches);
        }

        [HttpPost]
        public async Task<ActionResult<Match>> CrearMatch(int usuario1Id, int usuario2Id, int juegoId)
        {
            var match = await _matchingService.CrearMatchAsync(usuario1Id, usuario2Id, juegoId);
            return Ok(match);
        }

        [HttpPut("{matchId}/aceptar/{usuarioId}")]
        public async Task<IActionResult> AceptarMatch(int matchId, int usuarioId)
        {
            var resultado = await _matchingService.AceptarMatchAsync(matchId, usuarioId);

            if (!resultado)
            {
                return BadRequest("No se pudo aceptar el match. Verifica los datos e intenta de nuevo.");
            }

            return NoContent();
        }

        [HttpGet("pendientes/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Match>>> ObtenerMatchesPendientes(int usuarioId)
        {
            var matches = await _matchingService.ObtenerMatchesPendientesAsync(usuarioId);
            return Ok(matches);
        }

        [HttpGet("confirmados/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Match>>> ObtenerMatchesConfirmados(int usuarioId)
        {
            var matches = await _matchingService.ObtenerMatchesConfirmadosAsync(usuarioId);
            return Ok(matches);
        }
    }
}