namespace TeamFinder.Dtos
{
    public class ReputacionEstadisticasDto
    {
        public int UsuarioId { get; set; }
        public double PuntuacionPromedio { get; set; }
        public int TotalEvaluaciones { get; set; }
        public Dictionary<int, int> DistribucionPuntuaciones { get; set; }
        public int EvaluacionesEsteMes { get; set; }
        public double Tendencia { get; set; } // -1: bajando, 0: estable, 1: subiendo
    }
}
