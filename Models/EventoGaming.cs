using System.ComponentModel.DataAnnotations;

namespace TeamFinder.Api.Models
{
    public class EventoGaming
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(1000)]
        public string Descripcion { get; set; }

        [Required]
        public int JuegoId { get; set; }

        [Required]
        public int OrganizadorId { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        public int MaxParticipantes { get; set; }

        public string ImagenUrl { get; set; }

        public bool EsPublico { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public virtual Juego Juego { get; set; }
        public virtual Usuario Organizador { get; set; }
        public virtual ICollection<EventoParticipante> Participantes { get; set; }
    }
}