using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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
        [JsonIgnore]
        public string Contraseña { get; set; }

        [StringLength(50)]
        public string SteamId { get; set; }

        [Required]
        [StringLength(50)]
        public string Pais { get; set; }

        
        public int Edad { get; set; }

        [StringLength(250)]
        public string? Correo { get; set; }
        [StringLength(250)]
        public string? AvatarUrl { get; set; }


        [StringLength(20)]
        public string EstiloJuego { get; set; } // casual, pro, competitivo, etc.

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public virtual ICollection<SeguirUsuario> Siguiendo { get; set; }
        public virtual ICollection<SeguirUsuario> Seguidores { get; set; }
        public virtual ICollection<UsuarioJuego> Juegos { get; set; }
        public virtual ICollection<Mensaje> MensajesEnviados { get; set; }
        public virtual ICollection<Mensaje> MensajesRecibidos { get; set; }
        public virtual ICollection<Reputacion> EvaluacionesRecibidas { get; set; }
        public virtual ICollection<Reputacion> EvaluacionesRealizadas { get; set; }
        public virtual ICollection<UsuarioInsignia> Insignias { get; set; }
        public virtual ICollection<EventoGaming> EventosOrganizados { get; set; }
        public virtual ICollection<EventoParticipante> EventosParticipando { get; set; }
    }
}