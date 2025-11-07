using System.ComponentModel.DataAnnotations;

namespace TeamFinder.Api.Models
{
    public class PreferenciaMatching
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int JuegoId { get; set; }

        public string NivelHabilidad { get; set; } // Principiante, Intermedio, Avanzado

        public string Disponibilidad { get; set; } // Mañana, Tarde, Noche, Fin de semana

        public string Idioma { get; set; }

        public int EdadMinima { get; set; } = 0;

        public int EdadMaxima { get; set; } = 99;

        public bool SoloMicrófono { get; set; } = false;

        public string NotasAdicionales { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public virtual Usuario Usuario { get; set; }
        public virtual Juego Juego { get; set; }
    }
}