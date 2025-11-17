namespace TeamFinder.Dtos
{
    public class JuegoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public string ImagenUrl { get; set; }
        public int? SteamAppId { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
