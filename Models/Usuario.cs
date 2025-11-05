using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamFinder.Api.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Contraseña { get; set; }

        [StringLength(50)]
        public string SteamId { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public virtual ICollection<SeguirUsuario> Siguiendo { get; set; }
        public virtual ICollection<SeguirUsuario> Seguidores { get; set; }
        public virtual ICollection<UsuarioJuego> Juegos { get; set; }
        public virtual ICollection<Mensaje> MensajesEnviados { get; set; }
        public virtual ICollection<Mensaje> MensajesRecibidos { get; set; }

        // Propiedades de navegación para reputación
        public virtual ICollection<Reputacion> EvaluacionesRecibidas { get; set; }
        public virtual ICollection<Reputacion> EvaluacionesRealizadas { get; set; }
        public virtual ICollection<UsuarioInsignia> Insignias { get; set; }
    }
}