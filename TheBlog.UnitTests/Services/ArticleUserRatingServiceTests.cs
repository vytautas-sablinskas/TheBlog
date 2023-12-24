using AutoMapper;
using Moq;
using System.Linq.Expressions;
using TheBlog.Data.Database;
using TheBlog.Data.Entities;
using TheBlog.MVC.Services;
using TheBlog.MVC.ViewModels.Articles;

namespace TheBlog.UnitTests.Services
{
    public class ArticleUserRatingServiceTests
    {
        private Mock<IRepository<ArticleUserRating>> _mockArticleUserRatingRepository;
        private Mock<IRepository<Article>> _mockArticleRepository;
        private Mock<IMapper> _mockMapper;

        private ArticleUserRatingService _sut;

        [SetUp]
        public void SetUp()
        {
            _mockArticleUserRatingRepository = new Mock<IRepository<ArticleUserRating>>();
            _mockArticleRepository = new Mock<IRepository<Article>>();
            _mockMapper = new Mock<IMapper>();

            _sut = new ArticleUserRatingService(_mockArticleUserRatingRepository.Object, _mockArticleRepository.Object, _mockMapper.Object);
        }

        [TestCase(null, typeof(ArticleUserRatingViewModel))]
        [TestCase("validUserId", null)]
        public void AddRating_InvalidParameters_ReturnsFalse(string userId, Type viewModelType)
        {
            var viewModel = viewModelType == null ? null : (ArticleUserRatingViewModel)Activator.CreateInstance(viewModelType);

            var result = _sut.AddRating(userId, viewModel);

            Assert.That(result, Is.False);
        }

        [Test]
        public void AddRating_UserRatingFound_ReturnsFalse()
        {
            var queryableWithMember = new List<ArticleUserRating>
            {
                new ArticleUserRating()
            }.AsQueryable();

            _mockArticleUserRatingRepository
                 .Setup(m => m.FindByCondition(It.IsAny<Expression<Func<ArticleUserRating, bool>>>()))
                 .Returns(queryableWithMember);

            var result = _sut.AddRating("fakeid", new ArticleUserRatingViewModel());

            Assert.That(result, Is.False);
        }

        [Test]
        public void AddRating_ValidInputsAndUserNotFound_CallsMethodsAndReturnsTrue()
        {
            var emptyQueryable = new List<ArticleUserRating>().AsQueryable();
            var articleList = new List<Article>
            {
                new Article()
            }.AsQueryable();

            _mockArticleUserRatingRepository
                 .Setup(m => m.FindByCondition(It.IsAny<Expression<Func<ArticleUserRating, bool>>>()))
                 .Returns(emptyQueryable);
            _mockMapper.Setup(m => m.Map<ArticleUserRating>(It.IsAny<ArticleUserRatingViewModel>()))
                .Returns(new ArticleUserRating());
            _mockArticleRepository
                 .Setup(m => m.FindByCondition(It.IsAny<Expression<Func<Article, bool>>>()))
                 .Returns(articleList);

            var result = _sut.AddRating("fakeid", new ArticleUserRatingViewModel());

            _mockMapper.Verify(m => m.Map<ArticleUserRating>(It.IsAny<ArticleUserRatingViewModel>()), Times.Once);
            _mockArticleRepository.Verify(m => m.Update(It.IsAny<Article>()), Times.Once);
        }

        [TestCase(null, typeof(ArticleUserRatingViewModel))]
        [TestCase("validUserId", null)]
        public void UpdateRating_InvalidParameters_ReturnsFalse(string userId, Type viewModelType)
        {
            var viewModel = viewModelType == null ? null : (ArticleUserRatingViewModel)Activator.CreateInstance(viewModelType);

            var result = _sut.UpdateRating(userId, viewModel);

            Assert.That(result, Is.False);
        }

        [Test]
        public void UpdateRating_UserRatingNotFound_ReturnsFalse()
        {
            var emptyQueryable = new List<ArticleUserRating>().AsQueryable();

            _mockArticleUserRatingRepository
                .Setup(m => m.FindByCondition(It.IsAny<Expression<Func<ArticleUserRating, bool>>>()))
                .Returns(emptyQueryable);

            var result = _sut.UpdateRating("fakeid", new ArticleUserRatingViewModel());

            Assert.That(result, Is.False);
        }

        [Test]
        public void UpdateRating_ValidInputsAndUserRatingFound_CallsMethodsAndReturnsTrue()
        {
            var queryableWithItems = new List<ArticleUserRating>
            {
                new ArticleUserRating()
            }.AsQueryable();

            var articleList = new List<Article>
            {
                new Article()
            }.AsQueryable();

            _mockArticleUserRatingRepository
                 .Setup(m => m.FindByCondition(It.IsAny<Expression<Func<ArticleUserRating, bool>>>()))
                 .Returns(queryableWithItems);
            _mockMapper.Setup(m => m.Map<ArticleUserRating>(It.IsAny<ArticleUserRatingViewModel>()))
                .Returns(new ArticleUserRating());
            _mockArticleRepository
                 .Setup(m => m.FindByCondition(It.IsAny<Expression<Func<Article, bool>>>()))
                 .Returns(articleList);

            var result = _sut.UpdateRating("fakeid", new ArticleUserRatingViewModel());

            _mockArticleRepository.Verify(m => m.Update(It.IsAny<Article>()), Times.Once);
            _mockArticleUserRatingRepository.Verify(m => m.Update(It.IsAny<ArticleUserRating>()), Times.Once);
        }

        [TestCase(null, typeof(RemoveArticleUserRatingViewModel))]
        [TestCase("validUserId", null)]
        public void RemoveRating_InvalidParameters_ReturnsFalse(string userId, Type viewModelType)
        {
            var viewModel = viewModelType == null ? null : (RemoveArticleUserRatingViewModel)Activator.CreateInstance(viewModelType);

            var result = _sut.RemoveRating(userId, viewModel);

            Assert.That(result, Is.False);
        }

        [Test]
        public void RemoveRating_UserRatingNotFound_ReturnsFalse()
        {
            var emptyQueryable = new List<ArticleUserRating>().AsQueryable();

            _mockArticleUserRatingRepository
                .Setup(m => m.FindByCondition(It.IsAny<Expression<Func<ArticleUserRating, bool>>>()))
                .Returns(emptyQueryable);

            var result = _sut.RemoveRating("fakeid", new RemoveArticleUserRatingViewModel());

            Assert.That(result, Is.False);
        }

        [Test]
        public void RemoveRating_ValidInputsAndUserRatingFound_CallsMethodsAndReturnsTrue()
        {
            var queryableWithItems = new List<ArticleUserRating>
            {
                new ArticleUserRating()
            }.AsQueryable();

            var articleList = new List<Article>
            {
                new Article()
            }.AsQueryable();

            _mockArticleUserRatingRepository
                 .Setup(m => m.FindByCondition(It.IsAny<Expression<Func<ArticleUserRating, bool>>>()))
                 .Returns(queryableWithItems);
            _mockMapper.Setup(m => m.Map<ArticleUserRating>(It.IsAny<ArticleUserRatingViewModel>()))
                .Returns(new ArticleUserRating());
            _mockArticleRepository
                 .Setup(m => m.FindByCondition(It.IsAny<Expression<Func<Article, bool>>>()))
                 .Returns(articleList);

            var result = _sut.RemoveRating("fakeid", new RemoveArticleUserRatingViewModel());

            _mockArticleUserRatingRepository.Verify(m => m.Delete(It.IsAny<ArticleUserRating>()), Times.Once);
            _mockArticleRepository.Verify(m => m.Update(It.IsAny<Article>()), Times.Once);
        }
    }
}