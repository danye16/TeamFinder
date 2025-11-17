namespace TeamFinder.Dtos
{
    public class MatchDto
    {
        public int Id { get; set; }
        public int Usuario1Id { get; set; }
        public int Usuario2Id { get; set; }
        public int JuegoId { get; set; }
        public DateTime FechaMatch { get; set; }  
        public bool AceptadoPorUsuario1 { get; set; } 
        public bool AceptadoPorUsuario2 { get; set; }  
        public bool MatchConfirmado { get; set; }  
    }

}
