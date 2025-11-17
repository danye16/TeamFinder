using Microsoft.EntityFrameworkCore;
using TeamFinder.Api.Data;
using TeamFinder.Api.Models;

namespace TeamFinder.Api.Services
{
    public interface IGameEventsService
    {
        Task<IEnumerable<EventoGaming>> ObtenerEventosAsync();
        Task<EventoGaming> ObtenerEventoAsync(int eventoId);
        Task<EventoGaming> CrearEventoAsync(EventoGaming evento);
        Task<bool> ActualizarEventoAsync(EventoGaming evento);
        Task<bool> EliminarEventoAsync(int eventoId);
        Task<bool> UnirseEventoAsync(int eventoId, int usuarioId);
        Task<bool> AbandonarEventoAsync(int eventoId, int usuarioId);
        Task<bool> ConfirmarParticipacionAsync(int eventoId, int usuarioId, bool confirmado);
        Task<IEnumerable<EventoParticipante>> ObtenerParticipantesAsync(int eventoId);
        Task<IEnumerable<EventoGaming>> ObtenerEventosPorJuegoAsync(int juegoId);
        Task<IEnumerable<EventoGaming>> ObtenerEventosProximosAsync();
        Task<IEnumerable<EventoGaming>> ObtenerEventosOrganizadosPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<EventoGaming>> ObtenerEventosParticipandoPorUsuarioAsync(int usuarioId);

        // NUEVOS MÉTODOS PARA VALIDACIONES
        Task<bool> ExisteEventoAsync(int eventoId);
        Task<bool> ExisteUsuarioAsync(int usuarioId);
        Task<bool> ExisteJuegoAsync(int juegoId);
        Task<int> ObtenerCantidadParticipantesAsync(int eventoId);
    }

    public class GameEventsService : IGameEventsService
    {
        private readonly TeamFinderDbContext _context;

        public GameEventsService(TeamFinderDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventoGaming>> ObtenerEventosAsync()
        {
            return await _context.EventosGaming
                .Include(e => e.Juego)
                .Include(e => e.Organizador)
                .Include(e => e.Participantes)
                    .ThenInclude(p => p.Usuario)
                .OrderBy(e => e.FechaInicio)
                .ToListAsync();
        }

        public async Task<EventoGaming> ObtenerEventoAsync(int eventoId)
        {
            return await _context.EventosGaming
                .Include(e => e.Juego)
                .Include(e => e.Organizador)
                .Include(e => e.Participantes)
                    .ThenInclude(p => p.Usuario)
                .FirstOrDefaultAsync(e => e.Id == eventoId);
        }

        public async Task<EventoGaming> CrearEventoAsync(EventoGaming evento)
        {
            // Validar que el organizador existe
            var organizadorExiste = await _context.Usuarios.AnyAsync(u => u.Id == evento.OrganizadorId);
            if (!organizadorExiste)
            {
                throw new ArgumentException("El organizador no existe");
            }

            // Validar que el juego existe
            var juegoExiste = await _context.Juegos.AnyAsync(j => j.Id == evento.JuegoId);
            if (!juegoExiste)
            {
                throw new ArgumentException("El juego no existe");
            }

            // Validar fechas
            if (evento.FechaInicio >= evento.FechaFin)
            {
                throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin");
            }

            if (evento.FechaInicio <= DateTime.UtcNow)
            {
                throw new ArgumentException("La fecha de inicio debe ser futura");
            }

            _context.EventosGaming.Add(evento);
            await _context.SaveChangesAsync();
            return evento;
        }

        public async Task<bool> ActualizarEventoAsync(EventoGaming evento)
        {
            // Validar que el evento existe
            var eventoExistente = await _context.EventosGaming.FindAsync(evento.Id);
            if (eventoExistente == null)
            {
                return false;
            }

            // Validar fechas
            if (evento.FechaInicio >= evento.FechaFin)
            {
                throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin");
            }

            // No permitir actualizar el organizador
            evento.OrganizadorId = eventoExistente.OrganizadorId;
            evento.FechaCreacion = eventoExistente.FechaCreacion;

            _context.Entry(eventoExistente).CurrentValues.SetValues(evento);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventoExists(evento.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> EliminarEventoAsync(int eventoId)
        {
            var evento = await _context.EventosGaming
                .Include(e => e.Participantes)
                .FirstOrDefaultAsync(e => e.Id == eventoId);

            if (evento == null)
            {
                return false;
            }

            // Eliminar participantes primero
            if (evento.Participantes.Any())
            {
                _context.EventoParticipantes.RemoveRange(evento.Participantes);
            }

            _context.EventosGaming.Remove(evento);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnirseEventoAsync(int eventoId, int usuarioId)
        {
            // Verificar si el evento existe
            var evento = await _context.EventosGaming.FindAsync(eventoId);
            if (evento == null)
            {
                return false;
            }

            // Verificar si el usuario existe
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == usuarioId);
            if (!usuarioExiste)
            {
                return false;
            }

            // Verificar si el usuario ya está participando
            var participacionExistente = await _context.EventoParticipantes
                .AnyAsync(ep => ep.EventoId == eventoId && ep.UsuarioId == usuarioId);

            if (participacionExistente)
            {
                return false;
            }

            // Verificar si hay cupo disponible
            if (evento.MaxParticipantes > 0)
            {
                var participantesActuales = await _context.EventoParticipantes
                    .CountAsync(ep => ep.EventoId == eventoId);

                if (participantesActuales >= evento.MaxParticipantes)
                {
                    return false;
                }
            }

            // Verificar que el evento no haya comenzado
            if (evento.FechaInicio <= DateTime.UtcNow)
            {
                return false;
            }

            // Agregar al evento
            var participacion = new EventoParticipante
            {
                EventoId = eventoId,
                UsuarioId = usuarioId,
                FechaRegistro = DateTime.UtcNow,
                Confirmado = false
            };

            _context.EventoParticipantes.Add(participacion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AbandonarEventoAsync(int eventoId, int usuarioId)
        {
            var participacion = await _context.EventoParticipantes
                .FirstOrDefaultAsync(ep => ep.EventoId == eventoId && ep.UsuarioId == usuarioId);

            if (participacion == null)
            {
                return false;
            }

            // Verificar que el evento no haya comenzado
            var evento = await _context.EventosGaming.FindAsync(eventoId);
            if (evento?.FechaInicio <= DateTime.UtcNow)
            {
                return false; // No se puede abandonar un evento que ya comenzó
            }

            _context.EventoParticipantes.Remove(participacion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmarParticipacionAsync(int eventoId, int usuarioId, bool confirmado)
        {
            var participacion = await _context.EventoParticipantes
                .FirstOrDefaultAsync(ep => ep.EventoId == eventoId && ep.UsuarioId == usuarioId);

            if (participacion == null)
            {
                return false;
            }

            participacion.Confirmado = confirmado;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EventoParticipante>> ObtenerParticipantesAsync(int eventoId)
        {
            return await _context.EventoParticipantes
                .Where(ep => ep.EventoId == eventoId)
                .Include(ep => ep.Usuario)
                .Include(ep => ep.Evento)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventoGaming>> ObtenerEventosPorJuegoAsync(int juegoId)
        {
            return await _context.EventosGaming
                .Where(e => e.JuegoId == juegoId)
                .Include(e => e.Juego)
                .Include(e => e.Organizador)
                .Include(e => e.Participantes)
                    .ThenInclude(p => p.Usuario)
                .OrderBy(e => e.FechaInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventoGaming>> ObtenerEventosProximosAsync()
        {
            var ahora = DateTime.UtcNow;
            return await _context.EventosGaming
                .Where(e => e.FechaInicio > ahora)
                .Include(e => e.Juego)
                .Include(e => e.Organizador)
                .Include(e => e.Participantes)
                    .ThenInclude(p => p.Usuario)
                .OrderBy(e => e.FechaInicio)
                .Take(10) // Limitar a 10 eventos próximos
                .ToListAsync();
        }

        public async Task<IEnumerable<EventoGaming>> ObtenerEventosOrganizadosPorUsuarioAsync(int usuarioId)
        {
            return await _context.EventosGaming
                .Where(e => e.OrganizadorId == usuarioId)
                .Include(e => e.Juego)
                .Include(e => e.Organizador)
                .Include(e => e.Participantes)
                    .ThenInclude(p => p.Usuario)
                .OrderByDescending(e => e.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventoGaming>> ObtenerEventosParticipandoPorUsuarioAsync(int usuarioId)
        {
            return await _context.EventoParticipantes
                .Where(ep => ep.UsuarioId == usuarioId)
                .Include(ep => ep.Evento)
                    .ThenInclude(e => e.Juego)
                .Include(ep => ep.Evento)
                    .ThenInclude(e => e.Organizador)
                .Include(ep => ep.Evento)
                    .ThenInclude(e => e.Participantes)
                .Select(ep => ep.Evento)
                .OrderBy(e => e.FechaInicio)
                .ToListAsync();
        }

        // NUEVOS MÉTODOS AUXILIARES
        public async Task<bool> ExisteEventoAsync(int eventoId)
        {
            return await _context.EventosGaming.AnyAsync(e => e.Id == eventoId);
        }

        public async Task<bool> ExisteUsuarioAsync(int usuarioId)
        {
            return await _context.Usuarios.AnyAsync(u => u.Id == usuarioId);
        }

        public async Task<bool> ExisteJuegoAsync(int juegoId)
        {
            return await _context.Juegos.AnyAsync(j => j.Id == juegoId);
        }

        public async Task<int> ObtenerCantidadParticipantesAsync(int eventoId)
        {
            return await _context.EventoParticipantes
                .CountAsync(ep => ep.EventoId == eventoId);
        }

        private async Task<bool> EventoExists(int id)
        {
            return await _context.EventosGaming.AnyAsync(e => e.Id == id);
        }
    }
}