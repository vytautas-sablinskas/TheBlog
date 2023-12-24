using System.Diagnostics.CodeAnalysis;
using TheBlog.MVC.ViewModels.Articles;

namespace TheBlog.MVC.ViewModels.Home
{
    [ExcludeFromCodeCoverage]
    public class HomePageViewModel
    {
        public List<SimplifiedArticleViewModel> LastFiveArticles { get; set; }

        public List<SimplifiedArticleViewModel> TopThreeArticlesByRank { get; set; }

        public List<SimplifiedArticleViewModel> LastThreeCommentedArticles { get; set; }
    }
}