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


            // Configuración de PreferenciaMatching
            modelBuilder.Entity<PreferenciaMatching>()
                .HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PreferenciaMatching>()
                .HasOne(p => p.Juego)
                .WithMany()
                .HasForeignKey(p => p.JuegoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de Match
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
        }
    }
}