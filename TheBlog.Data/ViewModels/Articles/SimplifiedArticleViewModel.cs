using System.Diagnostics.CodeAnalysis;

namespace TheBlog.MVC.ViewModels.Articles
{
    [ExcludeFromCodeCoverage]
    public class SimplifiedArticleViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int Rating { get; set; }

        public bool LikedByUser { get; set; } = false;

        public bool DislikedByUser { get; set; } = false;
    }
}