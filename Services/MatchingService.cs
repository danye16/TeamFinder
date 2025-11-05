using Microsoft.EntityFrameworkCore;
using TeamFinder.Api.Data;
using TeamFinder.Api.Models;

namespace TeamFinder.Api.Services
{
    public interface IMatchingService
    {
        Task<IEnumerable<Usuario>> EncontrarMatchesAsync(int usuarioId, int juegoId);
        Task<Match> CrearMatchAsync(int usuario1Id, int usuario2Id, int juegoId);
        Task<bool> AceptarMatchAsync(int matchId, int usuarioId);
        Task<IEnumerable<Match>> ObtenerMatchesPendientesAsync(int usuarioId);
        Task<IEnumerable<Match>> ObtenerMatchesConfirmadosAsync(int usuarioId);
    }

    public class MatchingService : IMatchingService
    {
        private readonly TeamFinderDbContext _context;

        public MatchingService(TeamFinderDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> EncontrarMatchesAsync(int usuarioId, int juegoId)
        {
            // Obtener preferencias del usuario
            var preferenciasUsuario = await _context.PreferenciasMatching
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && p.JuegoId == juegoId);

            if (preferenciasUsuario == null)
            {
                return new List<Usuario>();
            }

            // Obtener usuarios que tienen el juego y no tienen un match confirmado con el usuario actual
            var usuariosConJuego = await _context.UsuarioJuegos
                .Where(uj => uj.JuegoId == juegoId && uj.UsuarioId != usuarioId)
                .Select(uj => uj.UsuarioId)
                .ToListAsync();

            var usuariosYaMatcheados = await _context.Matches
                .Where(m => (m.Usuario1Id == usuarioId || m.Usuario2Id == usuarioId) && m.MatchConfirmado)
                .Select(m => m.Usuario1Id == usuarioId ? m.Usuario2Id : m.Usuario1Id)
                .ToListAsync();

            var usuariosCandidatos = await _context.Usuarios
                .Where(u => usuariosConJuego.Contains(u.Id) && !usuariosYaMatcheados.Contains(u.Id))
                .ToListAsync();

            // Filtrar por preferencias
            var matches = usuariosCandidatos.Where(u =>
            {
                // Aquí implementarías la lógica de filtrado según las preferencias
                // Por simplicidad, devolvemos todos los candidatos

                // En un caso real, podrías verificar:
                // - Si el candidato tiene preferencias compatibles
                // - Si está en el rango de edad deseado
                // - Si tiene disponibilidad compatible
                // - Si habla el mismo idioma
                // - etc.

                return true;
            }).ToList();

            return matches;
        }

        public async Task<Match> CrearMatchAsync(int usuario1Id, int usuario2Id, int juegoId)
        {
            // Verificar si ya existe un match entre estos usuarios para este juego
            var matchExistente = await _context.Matches
                .FirstOrDefaultAsync(m =>
                    (m.Usuario1Id == usuario1Id && m.Usuario2Id == usuario2Id && m.JuegoId == juegoId) ||
                    (m.Usuario1Id == usuario2Id && m.Usuario2Id == usuario1Id && m.JuegoId == juegoId));

            if (matchExistente != null)
            {
                return matchExistente;
            }

            var nuevoMatch = new Match
            {
                Usuario1Id = usuario1Id,
                Usuario2Id = usuario2Id,
                JuegoId = juegoId,
                FechaMatch = DateTime.Now
            };

            _context.Matches.Add(nuevoMatch);
            await _context.SaveChangesAsync();

            return nuevoMatch;
        }

        public async Task<bool> AceptarMatchAsync(int matchId, int usuarioId)
        {
            var match = await _context.Matches.FindAsync(matchId);

            if (match == null)
            {
                return false;
            }

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

            // Si ambos usuarios aceptaron, confirmar el match
            if (match.AceptadoPorUsuario1 && match.AceptadoPorUsuario2)
            {
                match.MatchConfirmado = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Match>> ObtenerMatchesPendientesAsync(int usuarioId)
        {
            return await _context.Matches
                .Where(m =>
                    (m.Usuario1Id == usuarioId && !m.AceptadoPorUsuario1) ||
                    (m.Usuario2Id == usuarioId && !m.AceptadoPorUsuario2))
                .Include(m => m.Usuario1)
                .Include(m => m.Usuario2)
                .Include(m => m.Juego)
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> ObtenerMatchesConfirmadosAsync(int usuarioId)
        {
            return await _context.Matches
                .Where(m =>
                    (m.Usuario1Id == usuarioId || m.Usuario2Id == usuarioId) && m.MatchConfirmado)
                .Include(m => m.Usuario1)
                .Include(m => m.Usuario2)
                .Include(m => m.Juego)
                .ToListAsync();
        }
    }
}