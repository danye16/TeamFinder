namespace TeamFinder.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class MensajeActualizacionDto
    {
        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string Contenido { get; set; }

        public bool Leido { get; set; }
    }
}
