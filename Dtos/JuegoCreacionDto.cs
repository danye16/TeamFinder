using System.ComponentModel.DataAnnotations;
namespace TeamFinder.Dtos
{

    public class JuegoCreacionDto
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(50)]
        public string Categoria { get; set; }

        public string ImagenUrl { get; set; }

        public int? SteamAppId { get; set; }
    }
}
