namespace TeamFinder.Dtos
{
    public class ReputacionPromedioDto
    {
        public int UsuarioId { get; set; }
        public double PuntuacionPromedio { get; set; }
        public int TotalEvaluaciones { get; set; }
        public string Estrellas { get; set; }
    }
}
