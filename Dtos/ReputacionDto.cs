namespace TeamFinder.Dtos
{
    public class ReputacionDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int EvaluadorId { get; set; }
        public int Puntuacion { get; set; }
        public string Comentario { get; set; }
        public DateTime FechaEvaluacion { get; set; }
        public string EvaluadorUsername { get; set; }
        public string UsuarioUsername { get; set; }
    }
}
