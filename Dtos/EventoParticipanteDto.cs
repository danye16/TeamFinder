namespace TeamFinder.Dtos
{
    public class EventoParticipanteDto
    {
        public int Id { get; set; }
        public int EventoId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Confirmado { get; set; }
        public string UsuarioUsername { get; set; }

        public string NickEnEvento { get; set; }
        public string RolElegido { get; set; }
    }
}
