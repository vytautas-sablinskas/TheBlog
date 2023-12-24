namespace TheBlog.MVC.Services
{
    public class ReportedCommentViewModel
    {
        public int Id { get; set; }

        public int CommentId { get; set; }

        public string Reason { get; set; }

        public string Text { get; set; }
    }
}