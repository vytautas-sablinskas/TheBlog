using System.Diagnostics.CodeAnalysis;

namespace TheBlog.MVC.ViewModels.Articles
{
    [ExcludeFromCodeCoverage]
    public class ArticleViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public string? ImageBase64Encoded { get; set; }

        public string Author { get; set; }

        public List<ArticleCommentViewModel> Comments { get; set; }
    }
}