using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheBlog.MVC.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using TheBlog.Data.Utilities;
using TheBlog.MVC.ViewModels.Articles;

namespace TheBlog.API.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet]
        [Route("articles")]
        public IActionResult GetArticles([FromQuery] string? titleFilter)
        {
            var articles = _articleService.GetArticles(User.Identity.Name, titleFilter);

            return Ok(articles);
        }

        [HttpGet]
        [Route("articles/{articleId}")]
        public IActionResult GetArticle(int articleId)
        {
            var article = _articleService.GetArticle(articleId);
            if (article == null)
            {
                return BadRequest($"Article by id: '{articleId}' was not found");
            }

            return Ok(article);
        }

        [HttpPost]
        [Authorize(Roles = $"{AppRoleNames.Admin},{AppRoleNames.ArticleWriter}")]
        [Route("article")]
        public async Task<IActionResult> AddArticle([FromBody] AddArticleViewModel addArticleViewModel)
        {
            (var articleWasAdded, var articleId) = await _articleService.AddArticleAsync(addArticleViewModel, User.Identity.Name);
            if (!articleWasAdded)
            {
                return BadRequest("Problem adding article. Try again later!");
            }

            return CreatedAtAction(nameof(AddArticle), new { articleWasAdded, articleId });
        }

        [HttpPut]
        [Authorize]
        [Route("article")]
        public async Task<IActionResult> UpdateArticle([FromBody] EditArticleViewModel editArticleViewModel)
        {
            var articleWasEdited = await _articleService.EditArticleAsync(editArticleViewModel, User.Identity.Name);
            if (!articleWasEdited)
            {
                return BadRequest("Incorrect information provided or user is not creator of article.");
            }

            return NoContent();
        }

        [HttpDelete]
        [Authorize]
        [Route("article/{articleId}")]
        public async Task<IActionResult> DeleteArticle(int articleId)
        {
            var articleWasRemoved = await _articleService.RemoveArticleAsync(new RemoveArticleViewModel { Id = articleId },
                                                                             User.Identity.Name);
            if (!articleWasRemoved)
            {
                return BadRequest("Incorrect article id provided or user is not creator of the article.");
            }

            return NoContent();
        }
    }
}