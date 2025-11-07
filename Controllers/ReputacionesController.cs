using Microsoft.AspNetCore.Mvc;
using TeamFinder.Api.Services;
using TeamFinder.Api.Models;

namespace TeamFinder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReputacionesController : ControllerBase
    {
        private readonly IReputationService _reputationService;

        public ReputacionesController(IReputationService reputationService)
        {
            _reputationService = reputationService;
        }

        [HttpGet("puntuacion/{usuarioId}")]
        public async Task<ActionResult<double>> GetPuntuacionPromedio(int usuarioId)
        {
            var puntuacion = await _reputationService.CalcularPuntuacionPromedioAsync(usuarioId);
            return Ok(puntuacion);
        }

        [HttpGet("evaluaciones/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Reputacion>>> GetEvaluaciones(int usuarioId)
        {
            var evaluaciones = await _reputationService.ObtenerEvaluacionesAsync(usuarioId);
            return Ok(evaluaciones);
        }

        [HttpPost]
        public async Task<ActionResult<Reputacion>> PostEvaluacion(Reputacion evaluacion)
        {
            var nuevaEvaluacion = await _reputationService.AgregarEvaluacionAsync(evaluacion);
            return Ok(nuevaEvaluacion);
        }
    }
}