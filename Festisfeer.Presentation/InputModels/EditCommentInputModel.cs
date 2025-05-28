namespace Festisfeer.Presentation.InputModels
{
    public class EditCommentInputModel
    {
        public int CommentId { get; set; }
        public int FestivalId { get; set; }
        public string Content { get; set; }
    }
}