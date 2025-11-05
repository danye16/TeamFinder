using System.ComponentModel.DataAnnotations;

namespace TeamFinder.Api.Models
{
    public class Mensaje
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RemitenteId { get; set; }

        [Required]
        public int DestinatarioId { get; set; }

        [Required]
        public string Contenido { get; set; }

        public DateTime FechaEnvio { get; set; } = DateTime.Now;

        public bool Leido { get; set; } = false;

        // Propiedades de navegación
        public virtual Usuario Remitente { get; set; }
        public virtual Usuario Destinatario { get; set; }
    }
}