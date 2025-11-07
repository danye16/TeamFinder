using Microsoft.AspNetCore.Mvc;
using TeamFinder.Api.Services;
using TeamFinder.Api.Models;

namespace TeamFinder.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventosGamingController : ControllerBase
    {
        private readonly IGameEventsService _gameEventsService;

        public EventosGamingController(IGameEventsService gameEventsService)
        {
            _gameEventsService = gameEventsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoGaming>> GetEventos()
        {
            var eventos = await _gameEventsService.ObtenerEventosAsync();
            return Ok(eventos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventoGaming>> GetEvento(int id)
        {
            var evento = await _gameEventsService.ObtenerEventoAsync(id);

            if (evento == null)
            {
                return NotFound();
            }

            return Ok(evento);
        }

        [HttpPost]
        public async Task<ActionResult<EventoGaming>> PostEvento(EventoGaming evento)
        {
            var nuevoEvento = await _gameEventsService.CrearEventoAsync(evento);
            return CreatedAtAction(nameof(GetEvento), new { id = nuevoEvento.Id }, nuevoEvento);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvento(int id, EventoGaming evento)
        {
            if (id != evento.Id)
            {
                return BadRequest();
            }

            var resultado = await _gameEventsService.ActualizarEventoAsync(evento);

            if (!resultado)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvento(int id)
        {
            var resultado = await _gameEventsService.EliminarEventoAsync(id);

            if (!resultado)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("{eventoId}/unirse/{usuarioId}")]
        public async Task<IActionResult> UnirseEvento(int eventoId, int usuarioId)
        {
            var resultado = await _gameEventsService.UnirseEventoAsync(eventoId, usuarioId);

            if (!resultado)
            {
                return BadRequest("No se pudo unir al evento. Verifica si el evento existe, si ya estás participando o si hay cupo disponible.");
            }

            return NoContent();
        }

        [HttpDelete("{eventoId}/abandonar/{usuarioId}")]
        public async Task<IActionResult> AbandonarEvento(int eventoId, int usuarioId)
        {
            var resultado = await _gameEventsService.AbandonarEventoAsync(eventoId, usuarioId);

            if (!resultado)
            {
                return BadRequest("No se pudo abandonar el evento. Verifica si el evento existe o si estás participando en él.");
            }

            return NoContent();
        }

        [HttpPut("{eventoId}/confirmar/{usuarioId}")]
        public async Task<IActionResult> ConfirmarParticipacion(int eventoId, int usuarioId, [FromBody] bool confirmado)
        {
            var resultado = await _gameEventsService.ConfirmarParticipacionAsync(eventoId, usuarioId, confirmado);

            if (!resultado)
            {
                return BadRequest("No se pudo confirmar la participación. Verifica si el evento existe o si estás participando en él.");
            }

            return NoContent();
        }

        [HttpGet("{eventoId}/participantes")]
        public async Task<ActionResult<IEnumerable<EventoParticipante>>> GetParticipantes(int eventoId)
        {
            var participantes = await _gameEventsService.ObtenerParticipantesAsync(eventoId);
            return Ok(participantes);
        }

        [HttpGet("juego/{juegoId}")]
        public async Task<ActionResult<IEnumerable<EventoGaming>>> GetEventosPorJuego(int juegoId)
        {
            var eventos = await _gameEventsService.ObtenerEventosPorJuegoAsync(juegoId);
            return Ok(eventos);
        }

        [HttpGet("proximos")]
        public async Task<ActionResult<IEnumerable<EventoGaming>>> GetEventosProximos()
        {
            var eventos = await _gameEventsService.ObtenerEventosProximosAsync();
            return Ok(eventos);
        }

        [HttpGet("organizados/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<EventoGaming>>> GetEventosOrganizadosPorUsuario(int usuarioId)
        {
            var eventos = await _gameEventsService.ObtenerEventosOrganizadosPorUsuarioAsync(usuarioId);
            return Ok(eventos);
        }

        [HttpGet("participando/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<EventoGaming>>> GetEventosParticipandoPorUsuario(int usuarioId)
        {
            var eventos = await _gameEventsService.ObtenerEventosParticipandoPorUsuarioAsync(usuarioId);
            return Ok(eventos);
        }
    }
}