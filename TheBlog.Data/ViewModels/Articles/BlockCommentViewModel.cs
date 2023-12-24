using System.Diagnostics.CodeAnalysis;

namespace TheBlog.MVC.ViewModels.Articles
{
    [ExcludeFromCodeCoverage]
    public class BlockCommentViewModel
    {
        public int ReportId { get; set; }

        public int CommentId { get; set; }

        public bool BlockComment { get; set; }
    }
}