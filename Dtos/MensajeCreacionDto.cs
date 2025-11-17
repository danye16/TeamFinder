namespace TeamFinder.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class MensajeCreacionDto
    {
        [Required]
        public int RemitenteId { get; set; }

        [Required]
        public int DestinatarioId { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string Contenido { get; set; }
    }
}
