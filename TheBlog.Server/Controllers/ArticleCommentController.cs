using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheBlog.Data.Utilities;
using TheBlog.MVC.Services;
using TheBlog.MVC.ViewModels.Articles;

namespace TheBlog.MVC.Controllers
{
    public class ArticleCommentController : Controller
    {
        private readonly IArticleCommentService _articleCommentService;

        public ArticleCommentController(IArticleCommentService articleCommentService)
        {
            _articleCommentService = articleCommentService;
        }

        [HttpPost]
        [Authorize(Roles = $"{AppRoleNames.ArticleCommentator},{AppRoleNames.Admin}")]
        public IActionResult ArticleComment([FromBody] AddArticleCommentViewModel viewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            (var wasCreated, var commentId) = _articleCommentService.AddNewComment(userId, viewModel);

            return Json(new { success = wasCreated, commentId });
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> ArticleComment([FromBody] EditArticleCommentViewModel viewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wasEdited = await _articleCommentService.EditCommentAsync(userId, viewModel);

            return Json(new { success = wasEdited });
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> ArticleComment([FromQuery] RemoveArticleCommentViewModel viewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wasDeleted = await _articleCommentService.DeleteArticleCommentAsync(userId, viewModel);

            return Json(new { success = wasDeleted });
        }

        [HttpGet]
        [Authorize(Roles = AppRoleNames.Admin)]
        public IActionResult AllReportedComments()
        {
            var reportedComments = _articleCommentService.GetAllReportedComments();

            return Json(new { reportedComments });
        }

        [HttpGet]
        [Authorize]
        public IActionResult UserReportedCommentViewModel([FromQuery] int commentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userReportedCommentViewModel = _articleCommentService.GetUserReportedComment(userId, commentId);

            return Json(new { userReportedCommentViewModel });
        }

        [HttpPost]
        [Authorize]
        public IActionResult ReportComment([FromBody] AddOrUpdateReportCommentViewModel viewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wasReported = _articleCommentService.AddOrUpdateReportComment(userId, viewModel);

            return Json(new { success = wasReported });
        }

        [HttpPatch]
        [Authorize(Roles = AppRoleNames.Admin)]
        public IActionResult ChangeReportedCommentStatus([FromBody] BlockCommentViewModel viewModel)
        {
            var success = _articleCommentService.ChangeReportedCommentStatus(viewModel);

            return Json(new { success });
        }
    }
}