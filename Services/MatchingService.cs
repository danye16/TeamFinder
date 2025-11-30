using Microsoft.EntityFrameworkCore;
using TeamFinder.Api.Data;
using TeamFinder.Api.Models;

namespace TeamFinder.Api.Services
{
    public interface IMatchingService
    {
        // Métodos existentes que ya tienes
        Task<IEnumerable<Usuario>> EncontrarMatchesAsync(int usuarioId, int juegoId);
        Task<double> CalcularPorcentajeMatchAsync(PreferenciaMatching pref1, PreferenciaMatching pref2);
        Task<IEnumerable<MatchResult>> EncontrarMatchesDetalladosAsync(int usuarioId, int juegoId);

        // NUEVOS MÉTODOS que necesita el controlador
        Task<double> CalcularPorcentajeMatchAsync(int usuarioId1, int usuarioId2, int juegoId);
        Task<Match> CrearMatchAsync(int usuario1Id, int usuario2Id, int juegoId, int idIniciador);
        Task<bool> AceptarMatchAsync(int matchId, int usuarioId);
        Task<bool> RechazarMatchAsync(int matchId, int usuarioId);
        Task<IEnumerable<Match>> ObtenerMatchesPendientesAsync(int usuarioId);
        Task<IEnumerable<Match>> ObtenerMatchesConfirmadosAsync(int usuarioId);
        Task<Match> ObtenerMatchDetalleAsync(int matchId);
    }

    public class MatchResult
    {
        public Usuario Usuario { get; set; }
        public PreferenciaMatching PreferenciaMatching { get; set; }
        public double PorcentajeMatch { get; set; }
        public List<string> RazonesMatch { get; set; }
    }

    public class MatchingService : IMatchingService
    {
        private readonly TeamFinderDbContext _context;

        public MatchingService(TeamFinderDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MatchResult>> EncontrarMatchesDetalladosAsync(int usuarioId, int juegoId)
        {
            var usuarioPreferencia = await _context.PreferenciasMatching
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && p.JuegoId == juegoId);

            if (usuarioPreferencia == null)
                return Enumerable.Empty<MatchResult>();

            var otrasPreferencias = await _context.PreferenciasMatching
                .Include(p => p.Usuario)
                .Where(p => p.JuegoId == juegoId && p.UsuarioId != usuarioId)
                .ToListAsync();

            var matches = new List<MatchResult>();

            foreach (var otraPreferencia in otrasPreferencias)
            {
                var porcentajeMatch = await CalcularPorcentajeMatchAsync(usuarioPreferencia, otraPreferencia);

                if (porcentajeMatch >= 60) // Umbral mínimo de 60%
                {
                    matches.Add(new MatchResult
                    {
                        Usuario = otraPreferencia.Usuario,
                        PreferenciaMatching = otraPreferencia,
                        PorcentajeMatch = porcentajeMatch,
                        RazonesMatch = ObtenerRazonesMatch(usuarioPreferencia, otraPreferencia)
                    });
                }
            }

            return matches.OrderByDescending(m => m.PorcentajeMatch);
        }

        public async Task<double> CalcularPorcentajeMatchAsync(PreferenciaMatching pref1, PreferenciaMatching pref2)
        {
            double puntuacion = 0;
            double maxPuntuacion = 0;

            // 1. Nivel de habilidad (20%)
            maxPuntuacion += 20;
            if (pref1.NivelHabilidad == pref2.NivelHabilidad)
                puntuacion += 20;

            // 2. Estilo de juego (15%)
            maxPuntuacion += 15;
            if (pref1.EstiloJuego == pref2.EstiloJuego)
                puntuacion += 15;

            // 3. Disponibilidad (20%)
            maxPuntuacion += 20;
            if (pref1.DiasDisponibles == pref2.DiasDisponibles && pref1.HorarioDisponible == pref2.HorarioDisponible)
                puntuacion += 20;
            else if (TienenDisponibilidadCompatibilidad(pref1, pref2))
                puntuacion += 10;

            // 4. Geografía (15%)
            maxPuntuacion += 15;
            if (!pref1.MismoPais || pref1.Usuario.Pais == pref2.Usuario.Pais)
                puntuacion += 15;

            // 5. Idioma (10%)
            maxPuntuacion += 10;
            if (pref1.Idioma == pref2.Idioma)
                puntuacion += 10;

            // 6. Preferencias de comunicación (10%)
            maxPuntuacion += 10;
            if (pref1.ComunicacionVoz == pref2.ComunicacionVoz)
                puntuacion += 10;

            // 7. Rol preferido (10%)
            maxPuntuacion += 10;
            if (pref1.RolPreferido != pref2.RolPreferido) // Roles diferentes son mejor
                puntuacion += 10;

            return (puntuacion / maxPuntuacion) * 100;
        }

        private bool TienenDisponibilidadCompatibilidad(PreferenciaMatching pref1, PreferenciaMatching pref2)
        {
            // Lógica para determinar si las disponibilidades son compatibles
            // Por ejemplo, si ambos están disponibles en fines de semana
            return (pref1.DiasDisponibles?.Contains("FinDeSemana") == true &&
                    pref2.DiasDisponibles?.Contains("FinDeSemana") == true) ||
                   (pref1.HorarioDisponible == pref2.HorarioDisponible);
        }

        private List<string> ObtenerRazonesMatch(PreferenciaMatching pref1, PreferenciaMatching pref2)
        {
            var razones = new List<string>();

            if (pref1.NivelHabilidad == pref2.NivelHabilidad)
                razones.Add($"Mismo nivel de habilidad: {pref1.NivelHabilidad}");

            if (pref1.EstiloJuego == pref2.EstiloJuego)
                razones.Add($"Mismo estilo de juego: {pref1.EstiloJuego}");

            if (pref1.Idioma == pref2.Idioma)
                razones.Add($"Mismo idioma: {pref1.Idioma}");

            if (!pref1.MismoPais || pref1.Usuario.Pais == pref2.Usuario.Pais)
                razones.Add($"Mismo país: {pref1.Usuario.Pais}");

            if (pref1.RolPreferido != pref2.RolPreferido)
                razones.Add($"Roles complementarios: {pref1.RolPreferido} + {pref2.RolPreferido}");

            return razones;
        }

        public async Task<IEnumerable<Usuario>> EncontrarMatchesAsync(int usuarioId, int juegoId)
        {
            var matchesDetallados = await EncontrarMatchesDetalladosAsync(usuarioId, juegoId);
            return matchesDetallados.Select(m => m.Usuario);
        }


        public async Task<double> CalcularPorcentajeMatchAsync(int usuarioId1, int usuarioId2, int juegoId)
        {
            var pref1 = await _context.PreferenciasMatching
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId1 && p.JuegoId == juegoId);

            var pref2 = await _context.PreferenciasMatching
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId2 && p.JuegoId == juegoId);

            if (pref1 == null || pref2 == null)
                return 0;

            return await CalcularPorcentajeMatchAsync(pref1, pref2);
        }

        public async Task<Match> CrearMatchAsync(int usuario1Id, int usuario2Id, int juegoId, int idIniciador)
        {
            // 1. BUSCAR SI YA EXISTE (Importante para no duplicar y evitar errores en el chat)
            var matchExistente = await _context.Matches
                .Include(m => m.Usuario1)
                .Include(m => m.Usuario2)
                .FirstOrDefaultAsync(m =>
                    (m.Usuario1Id == usuario1Id && m.Usuario2Id == usuario2Id && m.JuegoId == juegoId) ||
                    (m.Usuario1Id == usuario2Id && m.Usuario2Id == usuario1Id && m.JuegoId == juegoId));

            // Si ya existe, lo devolvemos para que el chat cargue
            if (matchExistente != null)
            {
                return matchExistente;
            }

            // 2. Si no existe, validamos usuarios para crear uno nuevo
            var usuario1Existe = await _context.Usuarios.AnyAsync(u => u.Id == usuario1Id);
            var usuario2Existe = await _context.Usuarios.AnyAsync(u => u.Id == usuario2Id);
            var juegoExiste = await _context.Juegos.AnyAsync(j => j.Id == juegoId);

            if (!usuario1Existe || !usuario2Existe || !juegoExiste)
            {
                return null;
            }

            var match = new Match
            {
                Usuario1Id = usuario1Id,
                Usuario2Id = usuario2Id,
                JuegoId = juegoId,
                FechaMatch = DateTime.UtcNow,

                // LÓGICA CORREGIDA CON 'idIniciador':
                // Marca como aceptado SOLO al usuario que inició la acción.
                AceptadoPorUsuario1 = (usuario1Id == idIniciador),
                AceptadoPorUsuario2 = (usuario2Id == idIniciador),

                MatchConfirmado = false
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();
            return match;
        }

        public async Task<bool> AceptarMatchAsync(int matchId, int usuarioId)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
                return false;

            if (match.Usuario1Id == usuarioId)
            {
                match.AceptadoPorUsuario1 = true;  
            }
            else if (match.Usuario2Id == usuarioId)
            {
                match.AceptadoPorUsuario2 = true; 
            }
            else
            {
                return false; 
            }

            // Actualizar MatchConfirmado si ambos han aceptado
            match.MatchConfirmado = match.AceptadoPorUsuario1 && match.AceptadoPorUsuario2;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RechazarMatchAsync(int matchId, int usuarioId)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
                return false;

            // Si el usuario rechaza, eliminamos el match
            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Match>> ObtenerMatchesPendientesAsync(int usuarioId)
        {
            return await _context.Matches
                .Include(m => m.Usuario1) // <--- IMPORTANTE: Incluir datos
                .Include(m => m.Usuario2) // <--- IMPORTANTE: Incluir datos
                .Include(m => m.Juego)    // Opcional, para mostrar el icono del juego
                .Where(m => (m.Usuario1Id == usuarioId && !m.AceptadoPorUsuario1) ||
                           (m.Usuario2Id == usuarioId && !m.AceptadoPorUsuario2))
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> ObtenerMatchesConfirmadosAsync(int usuarioId)
        {
            return await _context.Matches
                .Include(m => m.Usuario1)
                .Include(m => m.Usuario2)
                .Include(m => m.Juego)
                .Where(m => (m.Usuario1Id == usuarioId && m.MatchConfirmado) ||
                           (m.Usuario2Id == usuarioId && m.MatchConfirmado))
                .ToListAsync();
        }

        public async Task<Match> ObtenerMatchDetalleAsync(int matchId)
        {
            return await _context.Matches
                .Include(m => m.Usuario1)
                .Include(m => m.Usuario2)
                .Include(m => m.Juego)
                .FirstOrDefaultAsync(m => m.Id == matchId);
        }
    }


}

    
