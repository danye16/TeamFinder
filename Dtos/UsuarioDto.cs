namespace TeamFinder.Dtos
{


    //PARA EL SISTEMA DE MATCHING
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Pais { get; set; }
        public int Edad { get; set; }
        public string EstiloJuego { get; set; }

        public string? AvatarUrl { get; set; }
        public string? Correo { get; set; }
    }
}
