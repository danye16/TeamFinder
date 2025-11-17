// UsuarioActualizacionDto.cs
using System.ComponentModel.DataAnnotations;

namespace TeamFinder.Shared.Dtos
{
    public class UsuarioActualizacionDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [StringLength(50)]
        public string SteamId { get; set; }

        [Required]
        [StringLength(50)]
        public string Pais { get; set; }

        [Required]
        [Range(1, 100)]
        public int Edad { get; set; }

        [Required]
        [StringLength(20)]
        public string EstiloJuego { get; set; }

        [StringLength(100)]
        public string NuevaContraseña { get; set; }
    }
}