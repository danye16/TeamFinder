using System.ComponentModel.DataAnnotations;

namespace TeamFinder.Dtos
{

    public class EventoGamingCreacionDto
    {
        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required]
        public int JuegoId { get; set; }

        [Required]
        public int OrganizadorId { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Required]
        [Range(1, 1000)]
        public int MaxParticipantes { get; set; }

        public string ImagenUrl { get; set; }

        public bool EsPublico { get; set; } = true;
    }
}
