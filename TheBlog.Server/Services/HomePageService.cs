using AutoMapper;
using TheBlog.MVC.ViewModels.Articles;
using TheBlog.MVC.ViewModels.Home;

namespace TheBlog.MVC.Services
{
    public class HomePageService : IHomePageService
    {
        private readonly IArticleFilteringService _articleFilteringService;
        private readonly IMapper _mapper;

        public HomePageService(IArticleFilteringService articleFilteringService, IMapper mapper)
        {
            _articleFilteringService = articleFilteringService;
            _mapper = mapper;
        }

        public HomePageViewModel GetHomePageArticles()
        {
            var lastFiveArticles = _articleFilteringService.GetLastFiveArticlesCreated();
            var topThreeArticlesByRating = _articleFilteringService.GetLastThreeArticlesByRatingDescending();
            var lastThreeCommentedArticles = _articleFilteringService.GetLastThreeCommentedArticles();

            return new HomePageViewModel
            {
                LastFiveArticles = lastFiveArticles.Select(_mapper.Map<SimplifiedArticleViewModel>).ToList(),
                TopThreeArticlesByRank = topThreeArticlesByRating.Select(_mapper.Map<SimplifiedArticleViewModel>).ToList(),
                LastThreeCommentedArticles = lastThreeCommentedArticles.Select(_mapper.Map<SimplifiedArticleViewModel>).ToList()
            };
        }
    }
}