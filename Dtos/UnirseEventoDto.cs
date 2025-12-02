using System.ComponentModel.DataAnnotations;

namespace TeamFinder.Dtos
{
    public class UnirseEventoDto
    {
        [Required]
        public int EventoId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public string NickEnEvento { get; set; }

        [Required]
        public string RolElegido { get; set; }
    }
}