using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheBlog.Data.Utilities;
using TheBlog.MVC.Services;
using TheBlog.MVC.ViewModels.Articles;

namespace TheBlog.MVC.Controllers
{
    [Authorize(Roles = AppRoleNames.ArticleRater)]
    public class ArticleUserRatingController : Controller
    {
        private readonly IArticleUserRatingService _articleUserRatingService;

        public ArticleUserRatingController(IArticleUserRatingService articleUserRatingService)
        {
            _articleUserRatingService = articleUserRatingService;
        }

        [HttpPost]
        public IActionResult Add([FromBody] ArticleUserRatingViewModel articleUserRatingViewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var articleRatingWasAdded = _articleUserRatingService.AddRating(userId, articleUserRatingViewModel);

            return Json(new { success = articleRatingWasAdded });
        }

        [HttpPut]
        public IActionResult Update([FromBody] ArticleUserRatingViewModel articleUserRatingViewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var articleRatingWasUpdated = _articleUserRatingService.UpdateRating(userId, articleUserRatingViewModel);

            return Json(new { success = articleRatingWasUpdated });
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] RemoveArticleUserRatingViewModel removeArticleUserRatingViewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var articleRatingWasDeleted = _articleUserRatingService.RemoveRating(userId, removeArticleUserRatingViewModel);

            return Json(new { success = articleRatingWasDeleted });
        }
    }
}