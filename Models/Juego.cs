using System.ComponentModel.DataAnnotations;

namespace TeamFinder.Api.Models
{
    public class Juego
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(50)]
        public string Categoria { get; set; }

        public string ImagenUrl { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public virtual ICollection<UsuarioJuego> Usuarios { get; set; }
    }
}