using System.ComponentModel.DataAnnotations;

namespace TeamFinder.Api.Models
{
    public class SeguirUsuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int SeguidoId { get; set; }

        public DateTime FechaSeguimiento { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public virtual Usuario Usuario { get; set; }
        public virtual Usuario Seguido { get; set; }
    }
}