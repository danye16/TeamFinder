using Microsoft.EntityFrameworkCore;
using TeamFinder.Api.Data;
using TeamFinder.Api.Models;

namespace TeamFinder.Api.Services
{
    public interface IReputationService
    {
        Task<double> CalcularPuntuacionPromedioAsync(int usuarioId);
        Task<IEnumerable<Reputacion>> ObtenerEvaluacionesAsync(int usuarioId);
        Task<Reputacion> AgregarEvaluacionAsync(Reputacion evaluacion);
        Task<IEnumerable<Insignia>> ObtenerInsigniasDisponiblesAsync();
        Task<IEnumerable<UsuarioInsignia>> ObtenerInsigniasUsuarioAsync(int usuarioId);
        Task VerificarYAsignarInsigniasAsync(int usuarioId);
    }

    public class ReputationService : IReputationService
    {
        private readonly TeamFinderDbContext _context;

        public ReputationService(TeamFinderDbContext context)
        {
            _context = context;
        }

        public async Task<double> CalcularPuntuacionPromedioAsync(int usuarioId)
        {
            var evaluaciones = await _context.Reputaciones
                .Where(r => r.UsuarioId == usuarioId)
                .ToListAsync();

            if (!evaluaciones.Any())
            {
                return 0;
            }

            return evaluaciones.Average(e => e.Puntuacion);
        }

        public async Task<IEnumerable<Reputacion>> ObtenerEvaluacionesAsync(int usuarioId)
        {
            return await _context.Reputaciones
                .Where(r => r.UsuarioId == usuarioId)
                .Include(r => r.Evaluador)
                .OrderByDescending(r => r.FechaEvaluacion)
                .ToListAsync();
        }

        public async Task<Reputacion> AgregarEvaluacionAsync(Reputacion evaluacion)
        {
            // Verificar si ya existe una evaluación de este evaluador a este usuario
            var evaluacionExistente = await _context.Reputaciones
                .FirstOrDefaultAsync(r => r.UsuarioId == evaluacion.UsuarioId && r.EvaluadorId == evaluacion.EvaluadorId);

            if (evaluacionExistente != null)
            {
                // Actualizar la evaluación existente
                evaluacionExistente.Puntuacion = evaluacion.Puntuacion;
                evaluacionExistente.Comentario = evaluacion.Comentario;
                evaluacionExistente.FechaEvaluacion = DateTime.Now;

                await _context.SaveChangesAsync();
                return evaluacionExistente;
            }
            else
            {
                // Crear una nueva evaluación
                _context.Reputaciones.Add(evaluacion);
                await _context.SaveChangesAsync();

                // Verificar si el usuario obtiene nuevas insignias
                await VerificarYAsignarInsigniasAsync(evaluacion.UsuarioId);

                return evaluacion;
            }
        }

        public async Task<IEnumerable<Insignia>> ObtenerInsigniasDisponiblesAsync()
        {
            return await _context.Insignias.ToListAsync();
        }

        public async Task<IEnumerable<UsuarioInsignia>> ObtenerInsigniasUsuarioAsync(int usuarioId)
        {
            return await _context.UsuarioInsignias
                .Where(ui => ui.UsuarioId == usuarioId)
                .Include(ui => ui.Insignia)
                .OrderBy(ui => ui.FechaObtencion)
                .ToListAsync();
        }

        public async Task VerificarYAsignarInsigniasAsync(int usuarioId)
        {
            // Obtener todas las insignias disponibles
            var insigniasDisponibles = await _context.Insignias.ToListAsync();

            // Obtener las insignias que el usuario ya tiene
            var insigniasUsuario = await _context.UsuarioInsignias
                .Where(ui => ui.UsuarioId == usuarioId)
                .Select(ui => ui.InsigniaId)
                .ToListAsync();

            // Calcular estadísticas del usuario
            var puntuacionPromedio = await CalcularPuntuacionPromedioAsync(usuarioId);
            var cantidadEvaluaciones = await _context.Reputaciones
                .CountAsync(r => r.UsuarioId == usuarioId);
            var cantidadMatches = await _context.Matches
                .CountAsync(m => (m.Usuario1Id == usuarioId || m.Usuario2Id == usuarioId) && m.MatchConfirmado);

            // Verificar cada insignia
            foreach (var insignia in insigniasDisponibles)
            {
                // Si el usuario ya tiene esta insignia, continuar con la siguiente
                if (insigniasUsuario.Contains(insignia.Id))
                {
                    continue;
                }

                // Verificar si el usuario cumple con los requisitos de la insignia
                bool cumpleRequisitos = false;

                if (insignia.RequisitoPuntuacion > 0 && puntuacionPromedio >= insignia.RequisitoPuntuacion)
                {
                    cumpleRequisitos = true;
                }

                if (insignia.RequisitoCantidadPartidas > 0 && cantidadMatches >= insignia.RequisitoCantidadPartidas)
                {
                    cumpleRequisitos = true;
                }

                // Si cumple con los requisitos, asignar la insignia
                if (cumpleRequisitos)
                {
                    var usuarioInsignia = new UsuarioInsignia
                    {
                        UsuarioId = usuarioId,
                        InsigniaId = insignia.Id,
                        FechaObtencion = DateTime.Now
                    };

                    _context.UsuarioInsignias.Add(usuarioInsignia);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}