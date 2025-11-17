using Microsoft.AspNetCore.Mvc;
using TeamFinder.Api.Models;
using TeamFinder.Api.Services;
using TeamFinder.Dtos;
using TeamFinder.Shared.Dtos;

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

        // GET: api/Reputaciones/PuntuacionPromedio/1
        [HttpGet("PuntuacionPromedio/{usuarioId}")]
        public async Task<ActionResult<ReputacionPromedioDto>> GetPuntuacionPromedio(int usuarioId)
        {
            var puntuacion = await _reputationService.CalcularPuntuacionPromedioAsync(usuarioId);
            var totalEvaluaciones = await _reputationService.ObtenerTotalEvaluacionesAsync(usuarioId);

            var resultado = new ReputacionPromedioDto
            {
                UsuarioId = usuarioId,
                PuntuacionPromedio = puntuacion,
                TotalEvaluaciones = totalEvaluaciones,
                Estrellas = ConvertirPuntuacionAEstrellas(puntuacion)
            };

            return Ok(resultado);
        }

        // GET: api/Reputaciones/Evaluaciones/1
        [HttpGet("Evaluaciones/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<ReputacionDto>>> GetEvaluaciones(int usuarioId)
        {
            var evaluaciones = await _reputationService.ObtenerEvaluacionesAsync(usuarioId);

            var evaluacionesDto = evaluaciones.Select(e => new ReputacionDto
            {
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                EvaluadorId = e.EvaluadorId,
                Puntuacion = e.Puntuacion,
                Comentario = e.Comentario,
                FechaEvaluacion = e.FechaEvaluacion,
                EvaluadorUsername = e.Evaluador.Username
            });

            return Ok(evaluacionesDto);
        }

        // GET: api/Reputaciones/EvaluacionesRealizadas/1
        [HttpGet("EvaluacionesRealizadas/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<ReputacionDto>>> GetEvaluacionesRealizadas(int usuarioId)
        {
            var evaluaciones = await _reputationService.ObtenerEvaluacionesRealizadasAsync(usuarioId);

            var evaluacionesDto = evaluaciones.Select(e => new ReputacionDto
            {
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                EvaluadorId = e.EvaluadorId,
                Puntuacion = e.Puntuacion,
                Comentario = e.Comentario,
                FechaEvaluacion = e.FechaEvaluacion,
                UsuarioUsername = e.Usuario.Username
            });

            return Ok(evaluacionesDto);
        }

        // GET: api/Reputaciones/Detalle/1
        [HttpGet("Detalle/{evaluacionId}")]
        public async Task<ActionResult<ReputacionDetalleDto>> GetEvaluacionDetalle(int evaluacionId)
        {
            var evaluacion = await _reputationService.ObtenerEvaluacionDetalleAsync(evaluacionId);

            if (evaluacion == null)
            {
                return NotFound();
            }

            var evaluacionDto = new ReputacionDetalleDto
            {
                Id = evaluacion.Id,
                UsuarioId = evaluacion.UsuarioId,
                EvaluadorId = evaluacion.EvaluadorId,
                Puntuacion = evaluacion.Puntuacion,
                Comentario = evaluacion.Comentario,
                FechaEvaluacion = evaluacion.FechaEvaluacion,
                UsuarioUsername = evaluacion.Usuario.Username,
                EvaluadorUsername = evaluacion.Evaluador.Username
            };

            return Ok(evaluacionDto);
        }

        // POST: api/Reputaciones/Crear
        [HttpPost("Crear")]
        public async Task<ActionResult<ReputacionDto>> PostEvaluacion(ReputacionCreacionDto evaluacionCreacionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar que no se esté evaluando a sí mismo
            if (evaluacionCreacionDto.UsuarioId == evaluacionCreacionDto.EvaluadorId)
            {
                return BadRequest("No puedes evaluarte a ti mismo.");
            }

            // Verificar que el usuario y evaluador existen
            var usuarioExiste = await _reputationService.ExisteUsuarioAsync(evaluacionCreacionDto.UsuarioId);
            var evaluadorExiste = await _reputationService.ExisteUsuarioAsync(evaluacionCreacionDto.EvaluadorId);

            if (!usuarioExiste || !evaluadorExiste)
            {
                return BadRequest("Usuario o evaluador no válido.");
            }

            // Verificar si ya existe una evaluación del mismo evaluador
            var evaluacionExistente = await _reputationService.ExisteEvaluacionAsync(
                evaluacionCreacionDto.UsuarioId,
                evaluacionCreacionDto.EvaluadorId);

            if (evaluacionExistente)
            {
                return BadRequest("Ya has evaluado a este usuario anteriormente.");
            }

            var evaluacion = new Reputacion
            {
                UsuarioId = evaluacionCreacionDto.UsuarioId,
                EvaluadorId = evaluacionCreacionDto.EvaluadorId,
                Puntuacion = evaluacionCreacionDto.Puntuacion,
                Comentario = evaluacionCreacionDto.Comentario,
                FechaEvaluacion = DateTime.UtcNow
            };

            var nuevaEvaluacion = await _reputationService.AgregarEvaluacionAsync(evaluacion);

            // Cargar datos relacionados para el DTO
            var evaluacionConDatos = await _reputationService.ObtenerEvaluacionDetalleAsync(nuevaEvaluacion.Id);

            var evaluacionDto = new ReputacionDto
            {
                Id = evaluacionConDatos.Id,
                UsuarioId = evaluacionConDatos.UsuarioId,
                EvaluadorId = evaluacionConDatos.EvaluadorId,
                Puntuacion = evaluacionConDatos.Puntuacion,
                Comentario = evaluacionConDatos.Comentario,
                FechaEvaluacion = evaluacionConDatos.FechaEvaluacion,
                EvaluadorUsername = evaluacionConDatos.Evaluador.Username
            };

            return CreatedAtAction(nameof(GetEvaluacionDetalle), new { evaluacionId = evaluacionDto.Id }, evaluacionDto);
        }

        // PUT: api/Reputaciones/Editar/1
        [HttpPut("Editar/{evaluacionId}")]
        public async Task<IActionResult> PutEvaluacion(int evaluacionId, ReputacionActualizacionDto evaluacionActualizacionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = await _reputationService.ActualizarEvaluacionAsync(
                evaluacionId,
                evaluacionActualizacionDto.Puntuacion,
                evaluacionActualizacionDto.Comentario);

            if (!resultado)
            {
                return NotFound();
            }

            return Ok(new { message = "Evaluación actualizada exitosamente" });
        }

        // DELETE: api/Reputaciones/Eliminar/1
        [HttpDelete("Eliminar/{evaluacionId}")]
        public async Task<IActionResult> DeleteEvaluacion(int evaluacionId)
        {
            var resultado = await _reputationService.EliminarEvaluacionAsync(evaluacionId);

            if (!resultado)
            {
                return NotFound();
            }

            return Ok(new { message = "Evaluación eliminada exitosamente" });
        }

        // GET: api/Reputaciones/Estadisticas/1
        [HttpGet("Estadisticas/{usuarioId}")]
        public async Task<ActionResult<ReputacionEstadisticasDto>> GetEstadisticas(int usuarioId)
        {
            var estadisticas = await _reputationService.ObtenerEstadisticasAsync(usuarioId);
            return Ok(estadisticas);
        }

        // Método auxiliar para convertir puntuación a estrellas
        private string ConvertirPuntuacionAEstrellas(double puntuacion)
        {
            return puntuacion switch
            {
                >= 4.5 => "⭐⭐⭐⭐⭐",
                >= 3.5 => "⭐⭐⭐⭐",
                >= 2.5 => "⭐⭐⭐",
                >= 1.5 => "⭐⭐",
                _ => "⭐"
            };
        }
    }
}