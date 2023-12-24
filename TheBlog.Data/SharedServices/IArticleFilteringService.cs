using TheBlog.Data.Entities;

namespace TheBlog.MVC.Services
{
    public interface IArticleFilteringService
    {
        List<Article> GetArticles(string? titleFilter);
        List<Article> GetLastFiveArticlesCreated();
        List<Article> GetLastThreeArticlesByRatingDescending();
        List<Article> GetLastThreeCommentedArticles();
    }
}