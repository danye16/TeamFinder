using Microsoft.EntityFrameworkCore;
using TeamFinder.Api.Data;
using TeamFinder.Api.Models;
using TeamFinder.Dtos;

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


        Task<int> ObtenerTotalEvaluacionesAsync(int usuarioId);
        Task<IEnumerable<Reputacion>> ObtenerEvaluacionesRealizadasAsync(int usuarioId);
        Task<Reputacion> ObtenerEvaluacionDetalleAsync(int evaluacionId);
        Task<bool> ExisteUsuarioAsync(int usuarioId);
        Task<bool> ExisteEvaluacionAsync(int usuarioId, int evaluadorId);
        Task<bool> ActualizarEvaluacionAsync(int evaluacionId, int puntuacion, string comentario);
        Task<bool> EliminarEvaluacionAsync(int evaluacionId);
        Task<ReputacionEstadisticasDto> ObtenerEstadisticasAsync(int usuarioId);
    }

    public class ReputationService : IReputationService
    {
        private readonly TeamFinderDbContext _context;

        public ReputationService(TeamFinderDbContext context)
        {
            _context = context;
        }

        // Métodos existentes (ya implementados)
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

        // NUEVOS MÉTODOS IMPLEMENTADOS

        public async Task<int> ObtenerTotalEvaluacionesAsync(int usuarioId)
        {
            return await _context.Reputaciones
                .CountAsync(r => r.UsuarioId == usuarioId);
        }

        public async Task<IEnumerable<Reputacion>> ObtenerEvaluacionesRealizadasAsync(int usuarioId)
        {
            return await _context.Reputaciones
                .Where(r => r.EvaluadorId == usuarioId)
                .Include(r => r.Usuario)
                .OrderByDescending(r => r.FechaEvaluacion)
                .ToListAsync();
        }

        public async Task<Reputacion> ObtenerEvaluacionDetalleAsync(int evaluacionId)
        {
            return await _context.Reputaciones
                .Include(r => r.Usuario)
                .Include(r => r.Evaluador)
                .FirstOrDefaultAsync(r => r.Id == evaluacionId);
        }

        public async Task<bool> ExisteUsuarioAsync(int usuarioId)
        {
            return await _context.Usuarios.AnyAsync(u => u.Id == usuarioId);
        }

        public async Task<bool> ExisteEvaluacionAsync(int usuarioId, int evaluadorId)
        {
            return await _context.Reputaciones
                .AnyAsync(r => r.UsuarioId == usuarioId && r.EvaluadorId == evaluadorId);
        }

        public async Task<bool> ActualizarEvaluacionAsync(int evaluacionId, int puntuacion, string comentario)
        {
            var evaluacion = await _context.Reputaciones.FindAsync(evaluacionId);
            if (evaluacion == null)
            {
                return false;
            }

            evaluacion.Puntuacion = puntuacion;
            evaluacion.Comentario = comentario;
            evaluacion.FechaEvaluacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Verificar si el usuario obtiene nuevas insignias después de la actualización
            await VerificarYAsignarInsigniasAsync(evaluacion.UsuarioId);

            return true;
        }

        public async Task<bool> EliminarEvaluacionAsync(int evaluacionId)
        {
            var evaluacion = await _context.Reputaciones.FindAsync(evaluacionId);
            if (evaluacion == null)
            {
                return false;
            }

            _context.Reputaciones.Remove(evaluacion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ReputacionEstadisticasDto> ObtenerEstadisticasAsync(int usuarioId)
        {
            var evaluaciones = await _context.Reputaciones
                .Where(r => r.UsuarioId == usuarioId)
                .ToListAsync();

            var totalEvaluaciones = evaluaciones.Count;
            var puntuacionPromedio = totalEvaluaciones > 0 ? evaluaciones.Average(e => e.Puntuacion) : 0;

            // Calcular distribución de puntuaciones
            var distribucion = new Dictionary<int, int>
            {
                { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }
            };

            foreach (var evaluacion in evaluaciones)
            {
                if (distribucion.ContainsKey(evaluacion.Puntuacion))
                {
                    distribucion[evaluacion.Puntuacion]++;
                }
            }

            // Calcular evaluaciones este mes
            var inicioMes = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var evaluacionesEsteMes = await _context.Reputaciones
                .CountAsync(r => r.UsuarioId == usuarioId && r.FechaEvaluacion >= inicioMes);

            // Calcular tendencia (simplificado: comparar con el mes anterior)
            var inicioMesAnterior = inicioMes.AddMonths(-1);
            var finMesAnterior = inicioMes.AddDays(-1);
            var evaluacionesMesAnterior = await _context.Reputaciones
                .CountAsync(r => r.UsuarioId == usuarioId &&
                                r.FechaEvaluacion >= inicioMesAnterior &&
                                r.FechaEvaluacion < inicioMes);

            double tendencia = 0;
            if (evaluacionesMesAnterior > 0)
            {
                tendencia = (evaluacionesEsteMes - evaluacionesMesAnterior) / (double)evaluacionesMesAnterior;
            }

            return new ReputacionEstadisticasDto
            {
                UsuarioId = usuarioId,
                PuntuacionPromedio = Math.Round(puntuacionPromedio, 2),
                TotalEvaluaciones = totalEvaluaciones,
                DistribucionPuntuaciones = distribucion,
                EvaluacionesEsteMes = evaluacionesEsteMes,
                Tendencia = tendencia
            };
        }
    }
}