using TheBlog.MVC.ViewModels.Articles;
using TheBlog.MVC.ViewModels.Home;

namespace TheBlog.MVC.Services
{
    public interface IArticleService
    {
        List<SimplifiedArticleViewModel> GetArticles(string username, string? filter = null);

        ArticleViewModel GetArticle(int articleId);

        Task<(bool success, int articleId)> AddArticleAsync(AddArticleViewModel addArticleViewModel, string creatorUsername);

        Task<bool> EditArticleAsync(EditArticleViewModel editArticleViewModel, string editorUsername);

        Task<bool> RemoveArticleAsync(RemoveArticleViewModel removeArticleViewModel, string removerUsername);
    }
}