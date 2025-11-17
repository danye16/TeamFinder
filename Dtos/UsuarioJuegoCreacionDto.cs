// UsuarioJuegoCreacionDto.cs
using System.ComponentModel.DataAnnotations;

public class UsuarioJuegoCreacionDto
{
    [Required]
    public int UsuarioId { get; set; }

    [Required]
    public int JuegoId { get; set; }
}