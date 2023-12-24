using AutoMapper;
using TheBlog.Data.Database;
using TheBlog.Data.Entities;
using TheBlog.Data.Utilities;
using TheBlog.MVC.ViewModels.Articles;

namespace TheBlog.MVC.Services
{
    public class ArticleCommentService : IArticleCommentService
    {
        private readonly IRepository<Article> _articleRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<ReportedComment> _reportedCommentRepository;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public ArticleCommentService(IRepository<Article> articleRepository, IRepository<Comment> commentRepository, IRepository<ReportedComment> reportedCommentRepository, IRoleService roleService, IMapper mapper)
        {
            _articleRepository = articleRepository;
            _commentRepository = commentRepository;
            _reportedCommentRepository = reportedCommentRepository;
            _roleService = roleService;
            _mapper = mapper;
        }

        public (bool success, int commentId) AddNewComment(string? userId, AddArticleCommentViewModel viewModel)
        {
            if (userId == null || viewModel == null)
            {
                return (success: false, commentId: -1);
            }

            var article = _articleRepository.FindByCondition(a => a.Id == viewModel.ArticleId)
                                            .FirstOrDefault();
            if (article == null)
            {
                return (success: false, commentId: -1);
            }

            var articleComment = _mapper.Map<Comment>(viewModel);
            articleComment.UserId = userId;

            article.Comments.Add(articleComment);
            _articleRepository.Update(article);

            return (success: true, commentId: articleComment.Id);
        }

        public async Task<bool> EditCommentAsync(string? userId, EditArticleCommentViewModel viewModel)
        {
            if (userId == null || viewModel == null)
            {
                return false;
            }

            var userIsAdmin = (await _roleService.GetUserRolesByUserIdAsync(userId))
                         .Any(r => r == AppRoleNames.Admin);

            var articleComment = _commentRepository.FindByCondition(c => c.Id == viewModel.CommentId &&
                                                             (c.UserId == userId || userIsAdmin)).FirstOrDefault();
            if (articleComment == null)
            {
                return false;
            }

            _mapper.Map(viewModel, articleComment);
            _commentRepository.Update(articleComment);

            return true;
        }

        public async Task<bool> DeleteArticleCommentAsync(string? userId, RemoveArticleCommentViewModel viewModel)
        {
            if (userId == null || viewModel == null)
            {
                return false;
            }

            var userIsAdmin = (await _roleService.GetUserRolesByUserIdAsync(userId))
                                                 .Select(r => r == AppRoleNames.Admin)
                                                 .FirstOrDefault();

            var articleComment = _commentRepository.FindByCondition(a => a.Id == viewModel.CommentId &&
                                                                   (a.UserId == userId || userIsAdmin)).FirstOrDefault();
            if (articleComment == null)
            {
                return false;
            }

            _commentRepository.Delete(articleComment);

            return true;
        }

        public List<ReportedCommentViewModel> GetAllReportedComments()
        {
            var commentReports = _reportedCommentRepository.GetAll()
                                                           .ToList();

            var commentReportIds = commentReports.Select(rc => rc.CommentId)
                                                 .ToList();

            var reportedComments = _commentRepository.FindByCondition(c => commentReportIds.Contains(c.Id))
                                                     .ToList();

            var reportedCommentsViewModel = commentReports.Join(reportedComments,
                                                          reportedComment => reportedComment.CommentId,
                                                          comment => comment.Id,
                                                          (reportedComment, comment) => new ReportedCommentViewModel
                                                          {
                                                              Id = reportedComment.Id,
                                                              CommentId = comment.Id,
                                                              Reason = reportedComment.Reason,
                                                              Text = comment.Text,
                                                          }).ToList();

            return reportedCommentsViewModel;
        }

        public ReportedCommentViewModel GetUserReportedComment(string? userId, int commentId)
        {
            if (userId == null || commentId < 0)
            {
                return null;
            }

            var userReportedComment = _reportedCommentRepository.FindByCondition(rc => rc.UserId == userId &&
                                                                                       rc.CommentId == commentId)
                                                                .FirstOrDefault();

            var reportedCommentViewModel = _mapper.Map<ReportedCommentViewModel>(userReportedComment);

            return reportedCommentViewModel;
        }

        public bool ChangeReportedCommentStatus(BlockCommentViewModel blockCommentViewModel)
        {
            var commentReports = _reportedCommentRepository.FindByCondition(c => c.CommentId == blockCommentViewModel.CommentId)
                                                           .ToList();
            if (commentReports == null)
            {
                return false;
            }

            if (blockCommentViewModel.BlockComment)
            {
                var comment = _commentRepository.FindByCondition(c => c.Id == blockCommentViewModel.CommentId)
                                                .FirstOrDefault();
                if (comment == null)
                {
                    return false;
                }

                comment.IsBlocked = true;
                _commentRepository.Update(comment);

                foreach (var report in commentReports)
                {
                    _reportedCommentRepository.Delete(report);
                }

                return true;
            }

            var selectedReport = commentReports.Find(c => c.Id == blockCommentViewModel.ReportId);
            if (selectedReport == null)
            {
                return false;
            }

            _reportedCommentRepository.Delete(selectedReport);

            return true;
        }

        public bool AddOrUpdateReportComment(string? userWhoReportsId, AddOrUpdateReportCommentViewModel reportCommentViewModel)
        {
            if (string.IsNullOrEmpty(userWhoReportsId) || reportCommentViewModel == null)
            {
                return false;
            }

            var reportedComment = _reportedCommentRepository.FindByCondition(c => c.CommentId == reportCommentViewModel.CommentId &&
                                                                             c.UserId == userWhoReportsId).FirstOrDefault();
            if (reportedComment != null)
            {
                reportedComment.Reason = reportCommentViewModel.Reason;
                _reportedCommentRepository.Update(reportedComment);

                return true;
            }

            var commentToReport = _mapper.Map<ReportedComment>(reportCommentViewModel);
            commentToReport.UserId = userWhoReportsId;
            _reportedCommentRepository.Create(commentToReport);

            return true;
        }
    }
}