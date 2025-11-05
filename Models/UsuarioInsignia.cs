using System.ComponentModel.DataAnnotations;

namespace TeamFinder.Api.Models
{
    public class UsuarioInsignia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int InsigniaId { get; set; }

        public DateTime FechaObtencion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public virtual Usuario Usuario { get; set; }
        public virtual Insignia Insignia { get; set; }
    }
}