namespace TeamFinder.Dtos
{
    public class EventoGamingDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int JuegoId { get; set; }
        public int OrganizadorId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int MaxParticipantes { get; set; }
        public string ImagenUrl { get; set; }
        public bool EsPublico { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Datos adicionales para la UI
        public string JuegoNombre { get; set; }
        public string OrganizadorUsername { get; set; }
        public int CantidadParticipantes { get; set; }
        public bool TieneCuposDisponibles => CantidadParticipantes < MaxParticipantes;
    }
}
