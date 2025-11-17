namespace TeamFinder.Dtos
{
    public class PreferenciaMatchingDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int JuegoId { get; set; }

        public string NivelHabilidad { get; set; }
        public string EstiloJuego { get; set; }

        public string DiasDisponibles { get; set; }
        public string HorarioDisponible { get; set; }
        public int HorasPorSesion { get; set; }

        public string PaisPreferido { get; set; }
        public bool MismoPais { get; set; }
        public string Idioma { get; set; }

        public string RolPreferido { get; set; }
        public bool MicrofonoRequerido { get; set; }
        public bool ComunicacionVoz { get; set; }

        public int EdadMinimaPreferida { get; set; }
        public int EdadMaximaPreferida { get; set; }

        public int HorasEnJuego { get; set; }
        public string RangoCompetitivo { get; set; }

        public string NotasAdicionales { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
