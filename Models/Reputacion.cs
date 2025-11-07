using System.ComponentModel.DataAnnotations;

namespace TeamFinder.Api.Models
{
    public class Reputacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int EvaluadorId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Puntuacion { get; set; } // 1 a 5 estrellas

        public string Comentario { get; set; }

        public DateTime FechaEvaluacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public virtual Usuario Usuario { get; set; }
        public virtual Usuario Evaluador { get; set; }
    }
}