using System.ComponentModel.DataAnnotations;

namespace TeamFinder.Api.Models
{
    //Matching revisado
    public class Match
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Usuario1Id { get; set; }

        [Required]
        public int Usuario2Id { get; set; }

        [Required]
        public int JuegoId { get; set; }

        public DateTime FechaMatch { get; set; } = DateTime.Now;

        public bool AceptadoPorUsuario1 { get; set; } = false;

        public bool AceptadoPorUsuario2 { get; set; } = false;

        public bool MatchConfirmado { get; set; } = false;

        // Propiedades de navegación
        public virtual Usuario Usuario1 { get; set; }
        public virtual Usuario Usuario2 { get; set; }
        public virtual Juego Juego { get; set; }
    }
}