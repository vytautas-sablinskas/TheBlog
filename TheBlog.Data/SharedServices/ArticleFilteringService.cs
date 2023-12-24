using Microsoft.EntityFrameworkCore;
using TheBlog.Data.Database;
using TheBlog.Data.Entities;

namespace TheBlog.MVC.Services
{
    public class ArticleFilteringService : IArticleFilteringService
    {
        private readonly IRepository<Article> _articleRepository;

        public ArticleFilteringService(IRepository<Article> articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public List<Article> GetArticles(string? titleFilter)
        {
            if (string.IsNullOrEmpty(titleFilter))
            {
                return _articleRepository.GetAll()
                                        .Include(a => a.User)
                                        .ToList();
            }

            return _articleRepository.GetAll()
                          .Where(a => a.Title.Trim().ToLower().Contains(titleFilter.Trim().ToLower()))
                          .Include(a => a.User)
                          .ToList();
        }

        public List<Article> GetLastFiveArticlesCreated()
        {
            return _articleRepository.GetAll()
                                    .OrderByDescending(a => a.CreatedTime)
                                    .Take(5)
                                    .Include(a => a.User)
                                    .ToList();
        }

        public List<Article> GetLastThreeArticlesByRatingDescending()
        {
            return _articleRepository.GetAll()
                                    .OrderByDescending(a => a.Rating)
                                    .Take(3)
                                    .Include(a => a.User)
                                    .ToList();
        }

        public List<Article> GetLastThreeCommentedArticles()
        {
            var latestArticleComments = _articleRepository.GetAll()
                                    .Include(a => a.Comments)
                                    .Where(a => a.Comments.Any(c => !c.IsBlocked))
                                    .Select(a => new
                                    {
                                        Article = a,
                                        LatestCommentId = a.Comments.Max(c => c.Id)
                                    });

            var lastThreeCommentedArticles = latestArticleComments
                                    .OrderByDescending(x => x.LatestCommentId)
                                    .Select(x => x.Article)
                                    .Take(3)
                                    .Include(a => a.User)
                                    .ToList();

            return lastThreeCommentedArticles;
        }
    }
}