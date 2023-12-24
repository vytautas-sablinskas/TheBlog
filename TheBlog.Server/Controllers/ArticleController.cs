using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBlog.Data.Utilities;
using TheBlog.MVC.Services;
using TheBlog.MVC.ViewModels.Articles;

namespace TheBlog.MVC.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet]
        public IActionResult Articles([FromQuery] string? titleFilter)
        {
            var articles = _articleService.GetArticles(User.Identity.Name, titleFilter);

            return Json(new { articles });
        }

        [HttpGet]
        public IActionResult Article([FromQuery] int articleId)
        {
            var article = _articleService.GetArticle(articleId);

            return Json(new { article });
        }

        [HttpPost]
        [Authorize(Roles = $"{AppRoleNames.Admin},{AppRoleNames.ArticleWriter}")]
        public async Task<IActionResult> Article([FromBody] AddArticleViewModel addArticleViewModel)
        {
            (var articleWasAdded, var articleId) = await _articleService.AddArticleAsync(addArticleViewModel, User.Identity.Name);

            return Json(new { articleWasAdded, articleId });
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> Article([FromBody] EditArticleViewModel editArticleViewModel)
        {
            var articleWasEdited = await _articleService.EditArticleAsync(editArticleViewModel, User.Identity.Name);

            return Json(new { articleWasEdited });
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Article([FromQuery] RemoveArticleViewModel removeArticleViewModel)
        {
            var articleWasRemoved = await _articleService.RemoveArticleAsync(removeArticleViewModel, User.Identity.Name);

            return Json(new { articleWasRemoved });
        }
    }
}