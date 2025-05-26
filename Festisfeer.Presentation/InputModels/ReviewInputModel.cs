namespace Festisfeer.Presentation.InputModels
{
    public class ReviewInputModel
    {
        public int Id { get; set; } 
        public string? Content { get; set; }
        public int Rating { get; set; }
        public int FestivalId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
    }
}