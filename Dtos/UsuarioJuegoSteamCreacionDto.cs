// UsuarioJuegoSteamCreacionDto.cs
using System.ComponentModel.DataAnnotations;

public class UsuarioJuegoSteamCreacionDto
{
    [Required]
    public int UsuarioId { get; set; }

    [Required]
    public int SteamAppId { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; }

    [StringLength(50)]
    public string Categoria { get; set; }

    public string ImagenUrl { get; set; }
}