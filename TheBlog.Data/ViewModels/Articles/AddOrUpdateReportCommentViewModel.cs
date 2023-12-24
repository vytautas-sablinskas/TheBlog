using System.Diagnostics.CodeAnalysis;

namespace TheBlog.MVC.ViewModels.Articles
{
    [ExcludeFromCodeCoverage]
    public class AddOrUpdateReportCommentViewModel
    {
        public int CommentId { get; set; }

        public string Reason { get; set; }
    }
}