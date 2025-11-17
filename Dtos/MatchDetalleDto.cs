namespace TeamFinder.Dtos
{
    public class MatchDetalleDto
    {
        public int Id { get; set; }
        public UsuarioDto Usuario1 { get; set; }
        public UsuarioDto Usuario2 { get; set; }
        public JuegoDto Juego { get; set; }
        public DateTime FechaMatch { get; set; }
        public bool AceptadoPorUsuario1 { get; set; }  
        public bool AceptadoPorUsuario2 { get; set; } 
        public bool MatchConfirmado { get; set; } 
    }
}
