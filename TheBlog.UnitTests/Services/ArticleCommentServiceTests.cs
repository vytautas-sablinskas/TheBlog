using AutoMapper;
using Moq;
using System.ComponentModel.Design;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using TheBlog.Data.Database;
using TheBlog.Data.Entities;
using TheBlog.Data.Utilities;
using TheBlog.MVC.Services;
using TheBlog.MVC.ViewModels.Articles;

namespace TheBlog.UnitTests.Services
{
    [TestFixture]
    public class ArticleCommentServiceTests
    {
        private Mock<IRepository<Article>> _mockArticleRepository;
        private Mock<IRepository<Comment>> _mockCommentRepository;
        private Mock<IRepository<ReportedComment>> _mockReportedCommentRepository;
        private Mock<IRoleService> _mockRoleService;
        private Mock<IMapper> _mockMapper;

        private ArticleCommentService _sut;

        [SetUp]
        public void SetUp()
        {
            _mockArticleRepository = new Mock<IRepository<Article>>();
            _mockCommentRepository = new Mock<IRepository<Comment>>();
            _mockReportedCommentRepository = new Mock<IRepository<ReportedComment>>();
            _mockRoleService = new Mock<IRoleService>();
            _mockMapper = new Mock<IMapper>();

            _sut = new ArticleCommentService(_mockArticleRepository.Object, _mockCommentRepository.Object, _mockReportedCommentRepository.Object, _mockRoleService.Object, _mockMapper.Object);
        }

        [TestCase(null, typeof(AddArticleCommentViewModel))]
        [TestCase("validUserId", null)]
        public void AddNewComment_InvalidParameters_ReturnsFalse(string userId, Type viewModelType)
        {
            var viewModel = viewModelType == null ? null : (AddArticleCommentViewModel)Activator.CreateInstance(viewModelType);

            (var success, var commentId) = _sut.AddNewComment(userId, viewModel);

            Assert.Multiple(() =>
            {
                Assert.That(success, Is.False);
                Assert.That(commentId, Is.EqualTo(-1));
            });
        }

        [Test]
        public void AddNewComment_ArticleNotFound_ReturnsFalse()
        {
            var emptyQueryable = new List<Article>().AsQueryable();

            _mockArticleRepository
                .Setup(m => m.FindByCondition(It.IsAny<Expression<Func<Article, bool>>>()))
                .Returns(emptyQueryable);

            (var success, var commentId) = _sut.AddNewComment("fakeid", new AddArticleCommentViewModel());

            Assert.Multiple(() =>
            {
                Assert.That(success, Is.False);
                Assert.That(commentId, Is.EqualTo(-1));
            });
        }

        [Test]
        public void AddNewComment_ValidInputs_ReturnsSuccess()
        {
            var article = new Article();
            article.Comments = new List<Comment>();

            var articleListFound = new List<Article>
            {
                article
            }.AsQueryable();

            _mockArticleRepository
                .Setup(m => m.FindByCondition(It.IsAny<Expression<Func<Article, bool>>>()))
                .Returns(articleListFound);
            _mockMapper.Setup(m => m.Map<Comment>(It.IsAny<AddArticleCommentViewModel>()))
                       .Returns(new Comment());

            (var success, var commentId) = _sut.AddNewComment("fakeid", new AddArticleCommentViewModel());

            Assert.That(success, Is.True);
        }

        [TestCase(null, typeof(EditArticleCommentViewModel))]
        [TestCase("validUserId", null)]
        public async Task EditCommentAsync_InvalidParameters_ReturnsFalse(string userId, Type viewModelType)
        {
            var viewModel = viewModelType == null ? null : (EditArticleCommentViewModel)Activator.CreateInstance(viewModelType);

            var success = await _sut.EditCommentAsync(userId, viewModel);

            Assert.That(success, Is.False);
        }

        [Test]
        public async Task EditCommentAsync_ArticleCommentNotFound_ReturnsFalse()
        {
            var emptyQueryable = new List<Comment>().AsQueryable();

            _mockRoleService.Setup(m => m.GetUserRolesByUserIdAsync(It.IsAny<string>()))
                            .ReturnsAsync(new List<string> { AppRoleNames.Admin });
            _mockCommentRepository.Setup(m => m.FindByCondition(It.IsAny<Expression<Func<Comment, bool>>>()))
                                  .Returns(emptyQueryable);

            var success = await _sut.EditCommentAsync("fakeId", new EditArticleCommentViewModel());

            Assert.That(success, Is.False);
        }

        [Test]
        public async Task EditCommentAsync_ValidInputs_ReturnsTrue()
        {
            var articleCommentList = new List<Comment>
            {
                new Comment()
            }.AsQueryable();

            _mockRoleService.Setup(m => m.GetUserRolesByUserIdAsync(It.IsAny<string>()))
                            .ReturnsAsync(new List<string> { });
            _mockCommentRepository.Setup(m => m.FindByCondition(It.IsAny<Expression<Func<Comment, bool>>>()))
                                  .Returns(articleCommentList);

            var success = await _sut.EditCommentAsync("fakeId", new EditArticleCommentViewModel());

            Assert.That(success, Is.True);
        }

        [TestCase(null, typeof(RemoveArticleCommentViewModel))]
        [TestCase("validUserId", null)]
        public async Task DeleteArticleCommentAsync_InvalidParameters_ReturnsFalse(string userId, Type viewModelType)
        {
            var viewModel = viewModelType == null ? null : (RemoveArticleCommentViewModel)Activator.CreateInstance(viewModelType);

            var success = await _sut.DeleteArticleCommentAsync(userId, viewModel);

            Assert.That(success, Is.False);
        }

        [Test]
        public async Task DeleteArticleCommentAsync_ArticleCommentNotFound_ReturnsFalse()
        {
            var emptyQueryable = new List<Comment>().AsQueryable();

            _mockRoleService.Setup(m => m.GetUserRolesByUserIdAsync(It.IsAny<string>()))
                            .ReturnsAsync(new List<string> { AppRoleNames.Admin });
            _mockCommentRepository.Setup(m => m.FindByCondition(It.IsAny<Expression<Func<Comment, bool>>>()))
                                  .Returns(emptyQueryable);

            var success = await _sut.DeleteArticleCommentAsync("fakeId", new RemoveArticleCommentViewModel());

            Assert.That(success, Is.False);
        }

        [Test]
        public async Task DeleteCommentAsync_ValidInputs_ReturnsTrue()
        {
            var articleCommentList = new List<Comment>
            {
                new Comment()
            }.AsQueryable();

            _mockRoleService.Setup(m => m.GetUserRolesByUserIdAsync(It.IsAny<string>()))
                            .ReturnsAsync(new List<string> { });
            _mockCommentRepository.Setup(m => m.FindByCondition(It.IsAny<Expression<Func<Comment, bool>>>()))
                                  .Returns(articleCommentList);

            var success = await _sut.DeleteArticleCommentAsync("fakeId", new RemoveArticleCommentViewModel());

            Assert.That(success, Is.True);
        }

        [Test]
        public void GetAllReportedComments_ReturnsReportedCommentsWithOriginalCommentText()
        {
            var commentReports = new List<ReportedComment>
            {
                new ReportedComment { CommentId = 1, Id = 1, Reason = "Report Reason 1" },
                new ReportedComment { CommentId = 2, Id = 2, Reason = "Report Reason 2" }
            }.AsQueryable();

            var reportedComments = new List<Comment>
            {
                new Comment { Id = 1, Text = "Comment Text 1"},
                new Comment { Id = 2, Text = "Comment Text 2"}
            }.AsQueryable();

            _mockReportedCommentRepository.Setup(m => m.GetAll())
                                          .Returns(commentReports);

            _mockCommentRepository.Setup(m => m.FindByCondition(It.IsAny<Expression<Func<Comment, bool>>>()))
                                  .Returns(reportedComments);

            var result = _sut.GetAllReportedComments();

            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].CommentId, Is.EqualTo(1));
            Assert.That(result[0].Reason, Is.EqualTo("Report Reason 1"));
            Assert.That(result[0].Text, Is.EqualTo("Comment Text 1"));

            Assert.That(result[1].Id, Is.EqualTo(2));
            Assert.That(result[1].CommentId, Is.EqualTo(2));
            Assert.That(result[1].Reason, Is.EqualTo("Report Reason 2"));
            Assert.That(result[1].Text, Is.EqualTo("Comment Text 2"));
        }
    }
}