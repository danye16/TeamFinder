namespace TeamFinder.Dtos
{
    public class MatchResultDto
    {
        public UsuarioDto Usuario { get; set; }
        public double PorcentajeMatch { get; set; }
        public List<string> RazonesMatch { get; set; }
        public string NivelHabilidad { get; set; }
        public string EstiloJuego { get; set; }
        public string RolPreferido { get; set; }
    }
}
