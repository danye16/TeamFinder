namespace TeamFinder.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class ReputacionActualizacionDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "La puntuación debe estar entre 1 y 5")]
        public int Puntuacion { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "El comentario debe tener entre 10 y 500 caracteres")]
        public string Comentario { get; set; }
    }
}
