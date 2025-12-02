using System.ComponentModel.DataAnnotations;

namespace TeamFinder.Api.Models
{
    public class EventoParticipante
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventoId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(50)]
        public string NickEnEvento { get; set; } 

        [Required]
        [StringLength(50)]
        public string RolElegido { get; set; } 

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public bool Confirmado { get; set; } = false;

        // Propiedades de navegación
        public virtual EventoGaming Evento { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}