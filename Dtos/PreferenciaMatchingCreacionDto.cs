namespace TeamFinder.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class PreferenciaMatchingCreacionDto
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int JuegoId { get; set; }

        // Nivel y estilo
        public string NivelHabilidad { get; set; }
        public string EstiloJuego { get; set; }

        // Disponibilidad
        public string DiasDisponibles { get; set; }
        public string HorarioDisponible { get; set; }
        public int HorasPorSesion { get; set; } = 2;

        // Geografía e idioma
        public string PaisPreferido { get; set; }
        public bool MismoPais { get; set; }
        public string Idioma { get; set; }

        // Preferencias de equipo
        public string RolPreferido { get; set; }
        public bool MicrofonoRequerido { get; set; }
        public bool ComunicacionVoz { get; set; } = true;

        // Rango de edad
        public int EdadMinimaPreferida { get; set; } = 18;
        public int EdadMaximaPreferida { get; set; } = 60;

        // Experiencia
        public int HorasEnJuego { get; set; }
        public string RangoCompetitivo { get; set; }

        public string NotasAdicionales { get; set; }
    }
}
