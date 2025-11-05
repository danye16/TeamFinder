using System.ComponentModel.DataAnnotations;

namespace TeamFinder.Api.Models
{
    public class UsuarioJuego
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int JuegoId { get; set; }

        public DateTime FechaAgregado { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public virtual Usuario Usuario { get; set; }
        public virtual Juego Juego { get; set; }
    }
}