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

        public DbSet<EventoGaming> EventosGaming { get; set; }
        public DbSet<EventoParticipante> EventoParticipantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la tabla intermedia SeguirUsuario
            modelBuilder.Entity<SeguirUsuario>()
                .HasKey(s => s.Id);

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

            // Configuración de la tabla intermedia UsuarioJuego
            modelBuilder.Entity<UsuarioJuego>()
                .HasKey(uj => uj.Id);

            modelBuilder.Entity<UsuarioJuego>()
                .HasOne(uj => uj.Usuario)
                .WithMany(u => u.Juegos)
                .HasForeignKey(uj => uj.UsuarioId);

            modelBuilder.Entity<UsuarioJuego>()
                .HasOne(uj => uj.Juego)
                .WithMany(j => j.Usuarios)
                .HasForeignKey(uj => uj.JuegoId);

            // Configuración de la tabla Mensaje
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


            // Configuración de EventoGaming
            modelBuilder.Entity<EventoGaming>()
                .HasOne(e => e.Juego)
                .WithMany()
                .HasForeignKey(e => e.JuegoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventoGaming>()
                .HasOne(e => e.Organizador)
                .WithMany(u => u.EventosOrganizados)
                .HasForeignKey(e => e.OrganizadorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de EventoParticipante
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

            // Asegurar que un usuario solo pueda participar una vez en un evento
            modelBuilder.Entity<EventoParticipante>()
                .HasIndex(ep => new { ep.EventoId, ep.UsuarioId })
                .IsUnique();
        
    }
    }
}