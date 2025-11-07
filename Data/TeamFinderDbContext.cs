using Microsoft.EntityFrameworkCore;
using TeamFinder.Api.Models;

namespace TeamFinder.Api.Data
{
    public class TeamFinderDbContext : DbContext
    {
        public TeamFinderDbContext(DbContextOptions<TeamFinderDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<SeguirUsuario> Seguimientos { get; set; }
        public DbSet<Juego> Juegos { get; set; }
        public DbSet<UsuarioJuego> UsuarioJuegos { get; set; }
        public DbSet<Mensaje> Mensajes { get; set; }

        public DbSet<PreferenciaMatching> PreferenciasMatching { get; set; }
        public DbSet<Match> Matches { get; set; }


        public DbSet<Reputacion> Reputaciones { get; set; }
        public DbSet<Insignia> Insignias { get; set; }
        public DbSet<UsuarioInsignia> UsuarioInsignias { get; set; }


        public DbSet<EventoGaming> EventosGaming { get; set; }
        public DbSet<EventoParticipante> EventoParticipantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- CONFIGURACIÓN DE EVENTOS GAMING ---
            // Le dice a EF que la propiedad de navegación 'Juego' usa la clave foránea 'JuegoId'.
            modelBuilder.Entity<EventoGaming>()
                .HasOne(e => e.Juego)
                .WithMany()
                .HasForeignKey(e => e.JuegoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventoGaming>()
                .HasOne(e => e.Organizador)
                .WithMany()
                .HasForeignKey(e => e.OrganizadorId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- CONFIGURACIÓN DE PREFERENCIAS MATCHING ---
            // Le dice a EF que la propiedad de navegación 'Juego' usa la clave foránea 'JuegoId'.
            modelBuilder.Entity<PreferenciaMatching>()
                .HasOne(p => p.Juego)
                .WithMany()
                .HasForeignKey(p => p.JuegoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PreferenciaMatching>()
                .HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- CONFIGURACIÓN DE MATCHES ---
            modelBuilder.Entity<Match>()
                .HasOne(m => m.Usuario1)
                .WithMany()
                .HasForeignKey(m => m.Usuario1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Usuario2)
                .WithMany()
                .HasForeignKey(m => m.Usuario2Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Juego)
                .WithMany()
                .HasForeignKey(m => m.JuegoId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- CONFIGURACIÓN DE SEGUIMIENTO ---
            modelBuilder.Entity<SeguirUsuario>()
                .HasOne(s => s.Usuario)
                .WithMany(u => u.Siguiendo)
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SeguirUsuario>()
                .HasOne(s => s.Seguido)
                .WithMany(u => u.Seguidores)
                .HasForeignKey(s => s.SeguidoId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- CONFIGURACIÓN DE USUARIO-JUEGO ---
            modelBuilder.Entity<UsuarioJuego>()
                .HasOne(uj => uj.Usuario)
                .WithMany(u => u.Juegos)
                .HasForeignKey(uj => uj.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsuarioJuego>()
                .HasOne(uj => uj.Juego)
                .WithMany(j => j.Usuarios)
                .HasForeignKey(uj => uj.JuegoId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- CONFIGURACIÓN DE MENSAJES ---
            modelBuilder.Entity<Mensaje>()
                .HasOne(m => m.Remitente)
                .WithMany(u => u.MensajesEnviados)
                .HasForeignKey(m => m.RemitenteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mensaje>()
                .HasOne(m => m.Destinatario)
                .WithMany(u => u.MensajesRecibidos)
                .HasForeignKey(m => m.DestinatarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- CONFIGURACIÓN DE REPUTACIÓN ---
            modelBuilder.Entity<Reputacion>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.EvaluacionesRecibidas)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reputacion>()
                .HasOne(r => r.Evaluador)
                .WithMany(u => u.EvaluacionesRealizadas)
                .HasForeignKey(r => r.EvaluadorId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- CONFIGURACIÓN DE INSIGNIAS ---
            modelBuilder.Entity<UsuarioInsignia>()
                .HasOne(ui => ui.Usuario)
                .WithMany(u => u.Insignias)
                .HasForeignKey(ui => ui.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsuarioInsignia>()
                .HasOne(ui => ui.Insignia)
                .WithMany(i => i.Usuarios)
                .HasForeignKey(ui => ui.InsigniaId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- CONFIGURACIÓN DE EVENTO-PARTICIPANTE ---
            modelBuilder.Entity<EventoParticipante>()
                .HasOne(ep => ep.Evento)
                .WithMany(e => e.Participantes)
                .HasForeignKey(ep => ep.EventoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventoParticipante>()
                .HasOne(ep => ep.Usuario)
                .WithMany(u => u.EventosParticipando)
                .HasForeignKey(ep => ep.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices únicos para evitar duplicados
            modelBuilder.Entity<SeguirUsuario>()
                .HasIndex(s => new { s.UsuarioId, s.SeguidoId })
                .IsUnique();

            modelBuilder.Entity<EventoParticipante>()
                .HasIndex(ep => new { ep.EventoId, ep.UsuarioId })
                .IsUnique();

            modelBuilder.Entity<Reputacion>()
                .HasIndex(r => new { r.UsuarioId, r.EvaluadorId })
                .IsUnique();
        }
    }
}