using Microsoft.AspNetCore.Mvc;
using TeamFinder.Api.Services;
using TeamFinder.Api.Models;

namespace TeamFinder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsigniasController : ControllerBase
    {
        private readonly IReputationService _reputationService;

        public InsigniasController(IReputationService reputationService)
        {
            _reputationService = reputationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Insignia>>> GetInsigniasDisponibles()
        {
            var insignias = await _reputationService.ObtenerInsigniasDisponiblesAsync();
            return Ok(insignias);
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<UsuarioInsignia>>> GetInsigniasUsuario(int usuarioId)
        {
            var insignias = await _reputationService.ObtenerInsigniasUsuarioAsync(usuarioId);
            return Ok(insignias);
        }

        [HttpPost("verificar/{usuarioId}")]
        public async Task<IActionResult> VerificarInsignias(int usuarioId)
        {
            await _reputationService.VerificarYAsignarInsigniasAsync(usuarioId);
            return NoContent();
        }
    }
}