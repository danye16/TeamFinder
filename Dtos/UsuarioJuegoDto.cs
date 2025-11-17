// UsuarioJuegoDto.cs
public class UsuarioJuegoDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int JuegoId { get; set; }
    public DateTime FechaAgregado { get; set; }

    // Datos básicos del usuario (sin las colecciones que causan ciclos)
    public string UsuarioUsername { get; set; }
    public string UsuarioPais { get; set; }
    public int UsuarioEdad { get; set; }
    public string UsuarioEstiloJuego { get; set; }

    // Datos básicos del juego (sin las colecciones que causan ciclos)
    public string JuegoNombre { get; set; }
    public string JuegoCategoria { get; set; }
    public string JuegoImagenUrl { get; set; }
}