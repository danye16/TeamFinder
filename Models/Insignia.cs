using System.ComponentModel.DataAnnotations;

namespace TeamFinder.Api.Models
{
    public class Insignia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        public string IconoUrl { get; set; }

        public string Color { get; set; } // Código de color hexadecimal

        public int RequisitoPuntuacion { get; set; } // Puntuación mínima para obtener la insignia

        public int RequisitoCantidadPartidas { get; set; } // Cantidad mínima de partidas

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public virtual ICollection<UsuarioInsignia> Usuarios { get; set; }
    }
}