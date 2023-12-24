using AutoMapper;
using TheBlog.Data.Database;
using TheBlog.Data.Entities;
using TheBlog.MVC.ViewModels.Articles;

namespace TheBlog.MVC.Services
{
    public class ArticleUserRatingService : IArticleUserRatingService
    {
        private readonly IRepository<ArticleUserRating> _userRatingRepository;
        private readonly IRepository<Article> _articleRepository;
        private readonly IMapper _mapper;

        public ArticleUserRatingService(IRepository<ArticleUserRating> userRatingRepository, IRepository<Article> articleRepository, IMapper mapper)
        {
            _userRatingRepository = userRatingRepository;
            _articleRepository = articleRepository;
            _mapper = mapper;
        }

        public bool AddRating(string? userId, ArticleUserRatingViewModel viewModel)
        {
            if (userId == null || viewModel == null)
            {
                return false;
            }

            var userRating = _userRatingRepository.FindByCondition(u => u.UserId == userId &&
                                                                              u.ArticleId == viewModel.ArticleId).FirstOrDefault();
            if (userRating != null)
            {
                return false;
            }

            var articleUserRating = _mapper.Map<ArticleUserRating>(viewModel);
            articleUserRating.UserId = userId;

            _userRatingRepository.Create(articleUserRating);

            var article = _articleRepository.FindByCondition(a => a.Id == viewModel.ArticleId).FirstOrDefault();
            article.Rating += CalculateRatingChangeForAdd(viewModel.LikedByUser, viewModel.DislikedByUser);
            _articleRepository.Update(article);

            return true;
        }

        public bool UpdateRating(string? userId, ArticleUserRatingViewModel viewModel)
        {
            if (userId == null || viewModel == null)
            {
                return false;
            }

            var userRating = _userRatingRepository.FindByCondition(u => u.UserId == userId &&
                                                                              u.ArticleId == viewModel.ArticleId).FirstOrDefault();
            if (userRating == null)
            {
                return false;
            }

            var article = _articleRepository.FindByCondition(a => a.Id == viewModel.ArticleId).FirstOrDefault();
            article.Rating += CalculateRatingChangeForUpdate(userRating.LikedByUser, userRating.DislikedByUser, viewModel.LikedByUser, viewModel.DislikedByUser);
            _articleRepository.Update(article);

            _mapper.Map(viewModel, userRating);
            _userRatingRepository.Update(userRating);

            return true;
        }

        public bool RemoveRating(string? userId, RemoveArticleUserRatingViewModel viewModel)
        {
            if (userId == null || viewModel == null)
            {
                return false;
            }

            var userRating = _userRatingRepository.FindByCondition(u => u.UserId == userId &&
                                                                              u.ArticleId == viewModel.ArticleId).FirstOrDefault();

            if (userRating == null)
            {
                return false;
            }

            _userRatingRepository.Delete(userRating);

            var article = _articleRepository.FindByCondition(a => a.Id == viewModel.ArticleId).FirstOrDefault();
            article.Rating += CalculateRatingChangeForRemove(userRating.LikedByUser, userRating.DislikedByUser);
            _articleRepository.Update(article);

            return true;
        }

        private int CalculateRatingChangeForAdd(bool likedByUser, bool dislikedByUser)
        {
            return likedByUser ? 1 : (dislikedByUser ? -1 : 0);
        }

        private int CalculateRatingChangeForUpdate(bool existingLikedByUser, bool existingDislikedByUser, bool newLikedByUser, bool newDislikedByUser)
        {
            int existingValue = existingLikedByUser ? 1 : (existingDislikedByUser ? -1 : 0);
            int newValue = newLikedByUser ? 1 : (newDislikedByUser ? -1 : 0);
            return newValue - existingValue;
        }

        private int CalculateRatingChangeForRemove(bool existingLikedByUser, bool existingDislikedByUser)
        {
            return existingLikedByUser ? -1 : (existingDislikedByUser ? 1 : 0);
        }
    }
}