using TheBlog.MVC.ViewModels.Articles;

namespace TheBlog.MVC.Services
{
    public interface IArticleUserRatingService
    {
        bool AddRating(string? userId, ArticleUserRatingViewModel viewModel);

        bool RemoveRating(string? userId, RemoveArticleUserRatingViewModel viewModel);

        bool UpdateRating(string? userId, ArticleUserRatingViewModel viewModel);
    }
}