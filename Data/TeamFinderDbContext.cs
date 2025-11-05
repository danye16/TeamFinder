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


        public DbSet<Reputacion> Reputaciones { get; set; }
        public DbSet<Insignia> Insignias { get; set; }
        public DbSet<UsuarioInsignia> UsuarioInsignias { get; set; }
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


            // Configuración de Reputacion
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

            // Configuración de UsuarioInsignia
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

            // Asegurar que un usuario solo pueda evaluar a otro una vez por match
            modelBuilder.Entity<Reputacion>()
                .HasIndex(r => new { r.UsuarioId, r.EvaluadorId })
                .IsUnique();
        }
    }
}