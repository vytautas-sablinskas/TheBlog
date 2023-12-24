using System.Diagnostics.CodeAnalysis;

namespace TheBlog.MVC.ViewModels.Articles
{
    [ExcludeFromCodeCoverage]
    public class ArticleCommentViewModel
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string Author { get; set; }
    }
}