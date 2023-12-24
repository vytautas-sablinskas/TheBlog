using TheBlog.Data.Entities;
using TheBlog.MVC.ViewModels.Articles;

namespace TheBlog.MVC.Services
{
    public interface IArticleCommentService
    {
        (bool success, int commentId) AddNewComment(string? userId, AddArticleCommentViewModel viewModel);

        Task<bool> EditCommentAsync(string? userId, EditArticleCommentViewModel viewModel);

        Task<bool> DeleteArticleCommentAsync(string? userId, RemoveArticleCommentViewModel viewModel);

        List<ReportedCommentViewModel> GetAllReportedComments();

        ReportedCommentViewModel GetUserReportedComment(string? userId, int commentId);

        bool ChangeReportedCommentStatus(BlockCommentViewModel blockCommentViewModel);

        bool AddOrUpdateReportComment(string? userWhoReportsId, AddOrUpdateReportCommentViewModel reportCommentViewModel);
    }
}