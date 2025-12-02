using Microsoft.AspNetCore.Mvc;
using TeamFinder.Api.Services;
using TeamFinder.Api.Models;
using TeamFinder.Shared.Dtos;
using TeamFinder.Dtos;

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

        // GET: api/EventosGaming/MostrarTodos
        [HttpGet("MostrarTodos")]
        public async Task<ActionResult<IEnumerable<EventoGamingDto>>> GetEventos()
        {
            var eventos = await _gameEventsService.ObtenerEventosAsync();
            var eventosDto = eventos.Select(e => MapToDto(e)).ToList();
            return Ok(eventosDto);
        }

        // GET: api/EventosGaming/BuscarPorId/5
        [HttpGet("BuscarPorId/{id}")]
        public async Task<ActionResult<EventoGamingDto>> GetEvento(int id)
        {
            var evento = await _gameEventsService.ObtenerEventoAsync(id);

            if (evento == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(evento));
        }

        // POST: api/EventosGaming/Crear
        [HttpPost("CrearEvento")]
        public async Task<ActionResult<EventoGamingDto>> PostEvento(EventoGamingCreacionDto eventoCreacionDto)
        {
            // Mapear DTO a entidad
            var evento = new EventoGaming
            {
                Titulo = eventoCreacionDto.Titulo,
                Descripcion = eventoCreacionDto.Descripcion,
                JuegoId = eventoCreacionDto.JuegoId,
                OrganizadorId = eventoCreacionDto.OrganizadorId,
                FechaInicio = eventoCreacionDto.FechaInicio,
                FechaFin = eventoCreacionDto.FechaFin,
                MaxParticipantes = eventoCreacionDto.MaxParticipantes,
                ImagenUrl = eventoCreacionDto.ImagenUrl,
                EsPublico = eventoCreacionDto.EsPublico,
                FechaCreacion = DateTime.UtcNow
            };

            var nuevoEvento = await _gameEventsService.CrearEventoAsync(evento);
            return CreatedAtAction(nameof(GetEvento), new { id = nuevoEvento.Id }, MapToDto(nuevoEvento));
        }

        // PUT: api/EventosGaming/Editar/5
        [HttpPut("EditarEvento/{id}")]
        public async Task<IActionResult> PutEvento(int id, EventoGamingActualizacionDto eventoActualizacionDto)
        {
            // Primero obtener el evento existente
            var eventoExistente = await _gameEventsService.ObtenerEventoAsync(id);
            if (eventoExistente == null)
            {
                return NotFound();
            }

            // Actualizar propiedades
            eventoExistente.Titulo = eventoActualizacionDto.Titulo;
            eventoExistente.Descripcion = eventoActualizacionDto.Descripcion;
            eventoExistente.JuegoId = eventoActualizacionDto.JuegoId;
            eventoExistente.FechaInicio = eventoActualizacionDto.FechaInicio;
            eventoExistente.FechaFin = eventoActualizacionDto.FechaFin;
            eventoExistente.MaxParticipantes = eventoActualizacionDto.MaxParticipantes;
            eventoExistente.ImagenUrl = eventoActualizacionDto.ImagenUrl;
            eventoExistente.EsPublico = eventoActualizacionDto.EsPublico;

            var resultado = await _gameEventsService.ActualizarEventoAsync(eventoExistente);

            if (!resultado)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/EventosGaming/Eliminar/5
        [HttpDelete("EliminarEvento/{id}")]
        public async Task<IActionResult> DeleteEvento(int id)
        {
            var resultado = await _gameEventsService.EliminarEventoAsync(id);

            if (!resultado)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/EventosGaming/Unirse
        [HttpPost("Unirse")]
        public async Task<IActionResult> UnirseEvento([FromBody] UnirseEventoDto dto)
        {
            // Validamos que vengan los datos
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Pasamos el Nick y Rol al servicio
            var resultado = await _gameEventsService.UnirseEventoAsync(
                dto.EventoId,
                dto.UsuarioId,
                dto.NickEnEvento,
                dto.RolElegido
            );

            if (!resultado)
            {
                return BadRequest("No se pudo unir al evento. Verifica cupos o si ya estás inscrito.");
            }

            return Ok(new { message = "Te has unido al evento exitosamente." });
        }

        [HttpGet("Participantes/{eventoId}")]
        public async Task<ActionResult<IEnumerable<EventoParticipanteDto>>> GetParticipantes(int eventoId)
        {
            var participantes = await _gameEventsService.ObtenerParticipantesAsync(eventoId);

            var dto = participantes.Select(p => new EventoParticipanteDto
            {
                Id = p.Id,
                EventoId = p.EventoId,
                UsuarioId = p.UsuarioId,
                FechaRegistro = p.FechaRegistro,
                Confirmado = p.Confirmado,
                UsuarioUsername = p.Usuario.Username,

                // Mapeamos los nuevos datos
                NickEnEvento = p.NickEnEvento,
                RolElegido = p.RolElegido
            }).ToList();

            return Ok(dto);
        }

        // DELETE: api/EventosGaming/Abandonar/5/1
        [HttpDelete("Abandonar/{eventoId}/{usuarioId}")]
        public async Task<IActionResult> AbandonarEvento(int eventoId, int usuarioId)
        {
            var resultado = await _gameEventsService.AbandonarEventoAsync(eventoId, usuarioId);

            if (!resultado)
            {
                return BadRequest("No se pudo abandonar el evento. Verifica si el evento existe o si estás participando en él.");
            }

            return Ok(new { message = "Has abandonado el evento exitosamente." });
        }

        // PUT: api/EventosGaming/Confirmar/5/1
        [HttpPut("Confirmar/{eventoId}/{usuarioId}")]
        public async Task<IActionResult> ConfirmarParticipacion(int eventoId, int usuarioId, [FromBody] bool confirmado)
        {
            var resultado = await _gameEventsService.ConfirmarParticipacionAsync(eventoId, usuarioId, confirmado);

            if (!resultado)
            {
                return BadRequest("No se pudo confirmar la participación. Verifica si el evento existe o si estás participando en él.");
            }

            return Ok(new { message = $"Participación {(confirmado ? "confirmada" : "cancelada")} exitosamente." });
        }

        // GET: api/EventosGaming/Participantes/5
        //[HttpGet("Participantes/{eventoId}")]
        //public async Task<ActionResult<IEnumerable<EventoParticipanteDto>>> GetParticipantes(int eventoId)
        //{
        //    var participantes = await _gameEventsService.ObtenerParticipantesAsync(eventoId);
        //    var participantesDto = participantes.Select(p => new EventoParticipanteDto
        //    {
        //        Id = p.Id,
        //        EventoId = p.EventoId,
        //        UsuarioId = p.UsuarioId,
        //        FechaRegistro = p.FechaRegistro,
        //        Confirmado = p.Confirmado,
        //        UsuarioUsername = p.Usuario.Username // Asumiendo que el servicio incluye el usuario
        //    }).ToList();
        //    return Ok(participantesDto);
        //}

        // GET: api/EventosGaming/Juego/1
        [HttpGet("Juego/{juegoId}")]
        public async Task<ActionResult<IEnumerable<EventoGamingDto>>> GetEventosPorJuego(int juegoId)
        {
            var eventos = await _gameEventsService.ObtenerEventosPorJuegoAsync(juegoId);
            var eventosDto = eventos.Select(e => MapToDto(e)).ToList();
            return Ok(eventosDto);
        }

        // GET: api/EventosGaming/Proximos
        [HttpGet("Proximos")]
        public async Task<ActionResult<IEnumerable<EventoGamingDto>>> GetEventosProximos()
        {
            var eventos = await _gameEventsService.ObtenerEventosProximosAsync();
            var eventosDto = eventos.Select(e => MapToDto(e)).ToList();
            return Ok(eventosDto);
        }

        // GET: api/EventosGaming/Organizados/1
        [HttpGet("Organizados/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<EventoGamingDto>>> GetEventosOrganizadosPorUsuario(int usuarioId)
        {
            var eventos = await _gameEventsService.ObtenerEventosOrganizadosPorUsuarioAsync(usuarioId);
            var eventosDto = eventos.Select(e => MapToDto(e)).ToList();
            return Ok(eventosDto);
        }

        // GET: api/EventosGaming/Participando/1
        [HttpGet("Participando/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<EventoGamingDto>>> GetEventosParticipandoPorUsuario(int usuarioId)
        {
            var eventos = await _gameEventsService.ObtenerEventosParticipandoPorUsuarioAsync(usuarioId);
            var eventosDto = eventos.Select(e => MapToDto(e)).ToList();
            return Ok(eventosDto);
        }

        // Método auxiliar para mapear EventoGaming a EventoGamingDto
        private EventoGamingDto MapToDto(EventoGaming evento)
        {
            return new EventoGamingDto
            {
                Id = evento.Id,
                Titulo = evento.Titulo,
                Descripcion = evento.Descripcion,
                JuegoId = evento.JuegoId,
                OrganizadorId = evento.OrganizadorId,
                FechaInicio = evento.FechaInicio,
                FechaFin = evento.FechaFin,
                MaxParticipantes = evento.MaxParticipantes,
                ImagenUrl = evento.ImagenUrl,
                EsPublico = evento.EsPublico,
                FechaCreacion = evento.FechaCreacion,
                // Asumiendo que el servicio incluye las propiedades de navegación
                JuegoNombre = evento.Juego?.Nombre,
                OrganizadorUsername = evento.Organizador?.Username,
                CantidadParticipantes = evento.Participantes?.Count ?? 0
            };
        }
    }
}