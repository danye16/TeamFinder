namespace TeamFinder.Dtos
{
    public class MensajeDto
    {
        public int Id { get; set; }
        public int RemitenteId { get; set; }
        public int DestinatarioId { get; set; }
        public string Contenido { get; set; }
        public DateTime FechaEnvio { get; set; }
        public bool Leido { get; set; }
        public string RemitenteUsername { get; set; }
        public string DestinatarioUsername { get; set; }
    }
}
