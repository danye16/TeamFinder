using System.ComponentModel.DataAnnotations;

namespace TeamFinder.Api.Models
{
    public class PreferenciaMatching
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int JuegoId { get; set; }

        // Nivel y estilo
        public string NivelHabilidad { get; set; } // "Principiante", "Intermedio", "Avanzado", "Experto"
        public string EstiloJuego { get; set; } // "Casual", "Competitivo", "Tryhard", "Relajado"

        // Disponibilidad - MEJORADO
        public string DiasDisponibles { get; set; } // "Lunes,Martes,Viernes" o "FinesDeSemana"
        public string HorarioDisponible { get; set; } // "Mañana", "Tarde", "Noche", "Madrugada"
        public int HorasPorSesion { get; set; } // 1, 2, 3+ horas

        // Geografía e idioma
        public string PaisPreferido { get; set; } // País específico o "Cualquiera"
        public bool MismoPais { get; set; } // Solo matching con mismo país
        public string Idioma { get; set; } // "Español", "Inglés", "Portugués", etc.

        // Preferencias de equipo
        public string RolPreferido { get; set; } // "DPS", "Support", "Tank", "Flex", etc.
        public bool MicrofonoRequerido { get; set; }
        public bool ComunicacionVoz { get; set; }

        // Rango de edad
        public int EdadMinimaPreferida { get; set; } = 18;
        public int EdadMaximaPreferida { get; set; } = 60;

        // Experiencia específica en el juego
        public int HorasEnJuego { get; set; } // Horas totales jugadas
        public string RangoCompetitivo { get; set; } // "Bronce", "Plata", "Oro", etc.

        public string NotasAdicionales { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public virtual Usuario Usuario { get; set; }
        public virtual Juego Juego { get; set; }
    }
}