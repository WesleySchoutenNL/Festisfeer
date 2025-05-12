namespace Festisfeer.Presentation.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
