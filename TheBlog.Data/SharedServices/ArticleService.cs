using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TheBlog.Data.Database;
using TheBlog.Data.Entities;
using TheBlog.Data.Utilities;
using TheBlog.MVC.ViewModels.Articles;
using TheBlog.MVC.Wrappers;

namespace TheBlog.MVC.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IRepository<Article> _articleRepository;
        private readonly IRepository<ArticleUserRating> _articleUserRatingRepository;
        private readonly IMapper _mapper;
        private readonly IUserManagerWrapper<User> _userManagerWrapper;
        private readonly IRoleService _roleService;
        private readonly IArticleFilteringService _articleFilteringService;

        public ArticleService(IRepository<Article> articleRepository, IRepository<ArticleUserRating> articleUserRatingRepository, IMapper mapper, IUserManagerWrapper<User> userManagerWrapper, IRoleService roleService, IArticleFilteringService articleFilteringService)
        {
            _articleRepository = articleRepository;
            _articleUserRatingRepository = articleUserRatingRepository;
            _mapper = mapper;
            _userManagerWrapper = userManagerWrapper;
            _roleService = roleService;
            _articleFilteringService = articleFilteringService;
        }

        public List<SimplifiedArticleViewModel> GetArticles(string username, string? titleFilter = null)
        {
            var articles = _articleFilteringService.GetArticles(titleFilter);

            return MapArticlesToSimplifiedArticleViewModels(articles, username);
        }

        private List<SimplifiedArticleViewModel> MapArticlesToSimplifiedArticleViewModels(List<Article> articles, string username)
        {
            var articleIds = articles.Select(a => a.Id).ToList();
            var userInteractions = _articleUserRatingRepository.GetAll()
                .Where(a => articleIds.Contains(a.ArticleId) && a.User.UserName == username)
                .Select(a => new { a.ArticleId, a.LikedByUser, a.DislikedByUser })
                .ToList();

            var articlesViewModelList = articles.Select(article =>
            {
                var viewModel = _mapper.Map<SimplifiedArticleViewModel>(article);
                var userInteraction = userInteractions.FirstOrDefault(ui => ui.ArticleId == viewModel.Id);

                if (userInteraction != null)
                {
                    viewModel.LikedByUser = userInteraction.LikedByUser;
                    viewModel.DislikedByUser = userInteraction.DislikedByUser;
                }

                return viewModel;
            }).ToList();

            return articlesViewModelList;
        }

        public ArticleViewModel GetArticle(int articleId)
        {
            var article = _articleRepository
                            .FindByCondition(a => a.Id == articleId)
                            .Include(a => a.Comments.Where(c => !c.IsBlocked))
                                .ThenInclude(c => c.User)
                            .Include(a => a.User)
                            .FirstOrDefault();

            return _mapper.Map<ArticleViewModel>(article);
        }

        public async Task<(bool success, int articleId)> AddArticleAsync(AddArticleViewModel addArticleViewModel, string creatorUsername)
        {
            if (addArticleViewModel == null)
            {
                return (success: false, articleId: -1);
            }

            if (!string.IsNullOrEmpty(addArticleViewModel.ImageBase64Encoded))
            {
                if (string.IsNullOrEmpty(addArticleViewModel.ImageType) ||
                    !ImageTypes.TypesAllowed.Contains(addArticleViewModel.ImageType))
                {
                    return (success: false, articleId: -1);
                }
            }

            var user = await _userManagerWrapper.FindByNameAsync(creatorUsername);
            if (user == null)
            {
                return (success: false, articleId: -1);
            }

            var article = _mapper.Map<Article>(addArticleViewModel);
            article.User = user;
            article.CreatedTime = DateTime.Now;

            article = _articleRepository.Create(article);

            return (success: true, articleId: article.Id);
        }

        public async Task<bool> EditArticleAsync(EditArticleViewModel editArticleViewModel, string editorUsername)
        {
            if (editArticleViewModel == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(editArticleViewModel.ImageBase64Encoded))
            {
                if (string.IsNullOrEmpty(editArticleViewModel.ImageType) ||
                    !ImageTypes.TypesAllowed.Contains(editArticleViewModel.ImageType))
                {
                    return false;
                }
            }

            var user = await _userManagerWrapper.FindByNameAsync(editorUsername);
            if (user == null)
            {
                return false;
            }

            var userIsAdmin = (await _roleService.GetUserRolesByUsernameAsync(editorUsername)).Contains(AppRoleNames.Admin);
            var article = _articleRepository
                .FindByCondition(a => (a.Id == editArticleViewModel.Id && a.User == user) ||
                                      (a.Id == editArticleViewModel.Id && userIsAdmin)).FirstOrDefault();

            if (article == null)
            {
                return false;
            }

            _mapper.Map(editArticleViewModel, article);
            _articleRepository.Update(article);

            return true;
        }

        public async Task<bool> RemoveArticleAsync(RemoveArticleViewModel removeArticleViewModel, string removerUsername)
        {
            var user = await _userManagerWrapper.FindByNameAsync(removerUsername);
            if (user == null)
            {
                return false;
            }

            var userIsAdmin = (await _roleService.GetUserRolesByUsernameAsync(removerUsername)).Contains(AppRoleNames.Admin);
            var article = _articleRepository
                .FindByCondition(a => (a.Id == removeArticleViewModel.Id && a.User == user) ||
                                      (a.Id == removeArticleViewModel.Id && userIsAdmin)).FirstOrDefault();
            if (article == null)
            {
                return false;
            }

            _articleRepository.Delete(article);

            return true;
        }
    }
}