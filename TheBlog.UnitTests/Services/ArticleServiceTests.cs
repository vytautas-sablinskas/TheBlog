using AutoMapper;
using Moq;
using System.Linq.Expressions;
using TheBlog.Data.Database;
using TheBlog.Data.Entities;
using TheBlog.Data.Utilities;
using TheBlog.MVC.Services;
using TheBlog.MVC.ViewModels.Articles;
using TheBlog.MVC.Wrappers;

namespace TheBlog.UnitTests.Services
{
    public class ArticleServiceTests
    {
        private Mock<IRepository<Article>> _mockArticleRepository;
        private Mock<IRepository<ArticleUserRating>> _mockArticleUserRatingRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<IUserManagerWrapper<User>> _mockUserManagerWrapper;
        private Mock<IRoleService> _mockRoleService;
        private Mock<IArticleFilteringService> _mockArticleFilteringService;

        private ArticleService _sut;

        [SetUp]
        public void SetUp()
        {
            _mockArticleRepository = new Mock<IRepository<Article>>();
            _mockArticleUserRatingRepository = new Mock<IRepository<ArticleUserRating>>();
            _mockMapper = new Mock<IMapper>();
            _mockUserManagerWrapper = new Mock<IUserManagerWrapper<User>>();
            _mockRoleService = new Mock<IRoleService>();
            _mockArticleFilteringService = new Mock<IArticleFilteringService>();

            _sut = new ArticleService(_mockArticleRepository.Object,
                                      _mockArticleUserRatingRepository.Object,
                                      _mockMapper.Object,
                                      _mockUserManagerWrapper.Object,
                                      _mockRoleService.Object,
                                      _mockArticleFilteringService.Object);
        }

        [Test]
        public void GetArticles_ShouldGetArticlesAndMapToSimplifiedArticleViewModels()
        {
            var userWhoTriesToGetArticles = "testUser";
            var user = new User
            {
                UserName = userWhoTriesToGetArticles
            };

            var articles = new List<Article>
            {
                new Article
                {
                    Id = 1,
                    User = user
                },
                new Article
                {
                    Id = 2,
                    User = user
                }
            };

            var mappedArticles = new List<SimplifiedArticleViewModel>
            {
                new SimplifiedArticleViewModel
                {
                    Id = 1,
                    Author = user.UserName
                },
                new SimplifiedArticleViewModel
                {
                    Id = 2,
                    Author = user.UserName
                }
            };

            var articleUserRatings = new List<ArticleUserRating>
            {
                new ArticleUserRating
                {
                    Id = 1,
                    User = user,
                    ArticleId = 1,
                    LikedByUser = false,
                    DislikedByUser = true
                },
                new ArticleUserRating
                {
                    Id = 2,
                    User = user,
                    ArticleId = 2,
                    LikedByUser = true,
                    DislikedByUser = false
                }
            }.AsQueryable();

            _mockArticleFilteringService.Setup(m => m.GetArticles(It.IsAny<string>()))
                                        .Returns(articles);
            _mockArticleUserRatingRepository.Setup(m => m.GetAll())
                                            .Returns(articleUserRatings);
            _mockMapper.SetupSequence(m => m.Map<SimplifiedArticleViewModel>(It.IsAny<Article>()))
                       .Returns(mappedArticles[0])
                       .Returns(mappedArticles[1]);

            var resultArticles = _sut.GetArticles(userWhoTriesToGetArticles, "");

            Assert.That(resultArticles, Has.Count.EqualTo(articles.Count));
            Assert.That(resultArticles[0].LikedByUser, Is.False);
            Assert.That(resultArticles[0].DislikedByUser, Is.True);
            Assert.That(resultArticles[1].LikedByUser, Is.True);
            Assert.That(resultArticles[1].DislikedByUser, Is.False);
        }

        [Test]
        public void GetArticle_ShouldReturnArticleWithComments()
        {
            var articles = new List<Article>
            {
                new Article
                {
                    Comments = new List<Comment>
                    {
                        new Comment
                        {
                            IsBlocked = true,
                            User = new User()
                        },
                        new Comment
                        {
                            IsBlocked = false,
                            User = new User
                            {
                                UserName = "Test1"
                            }
                        }
                    },
                    User = new User
                    {
                        UserName = "Test2"
                    }
                }
            };

            _mockArticleRepository.Setup(m => m.FindByCondition(It.IsAny<Expression<Func<Article, bool>>>()))
                                  .Returns(articles.AsQueryable());

            _mockMapper.Setup(m => m.Map<ArticleViewModel>(articles[0]))
                       .Returns(new ArticleViewModel());

            var articleViewModel = _sut.GetArticle(0);

            Assert.That(articleViewModel, Is.Not.Null);
        }

        [Test]
        public async Task AddArticleAsync_NullViewModelInput_ReturnsNoSuccess()
        {
            var result = await _sut.AddArticleAsync(null, "test");

            Assert.That(!result.success);
            Assert.That(result.articleId, Is.EqualTo(-1));
        }

        [TestCase("fakeImage", "wrongImageType")]
        [TestCase(null, "png")]
        [TestCase("fakeImage", null)]
        public async Task AddArticleAsync_ImageWrongFormat_ReturnsNoSuccess(string? imageBase64Encoded, string imageType)
        {
            var articleViewModel = new AddArticleViewModel
            {
                ImageBase64Encoded = imageBase64Encoded,
                ImageType = imageType
            };

            var result = await _sut.AddArticleAsync(articleViewModel, "fakeUser");

            Assert.That(!result.success);
            Assert.That(result.articleId, Is.EqualTo(-1));
        }

        [Test]
        public async Task AddArticleAsync_UserNotFound_ReturnsNoSuccess()
        {
            var articleViewModel = new AddArticleViewModel
            {
                ImageBase64Encoded = "fakeImage",
                ImageType = "png"
            };

            _mockUserManagerWrapper.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                                   .ReturnsAsync((User) null);

            var result = await _sut.AddArticleAsync(articleViewModel, "fakeUser");

            Assert.That(!result.success);
            Assert.That(result.articleId, Is.EqualTo(-1));
        }

        [Test]
        public async Task AddArticleAsync_ValidInputAndUserFound_ReturnsSuccess()
        {
            var articleViewModel = new AddArticleViewModel
            {
                ImageBase64Encoded = "fakeImage",
                ImageType = "png"
            };

            _mockUserManagerWrapper.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                                   .ReturnsAsync(new User());
            _mockMapper.Setup(m => m.Map<Article>(It.IsAny<AddArticleViewModel>()))
                       .Returns(new Article());
            _mockArticleRepository.Setup(m => m.Create(It.IsAny<Article>()))
                                  .Returns(new Article { Id = 2 });

            var result = await _sut.AddArticleAsync(articleViewModel, "fakeUser");

            Assert.That(result.success);
            Assert.That(result.articleId, Is.EqualTo(2));
        }

        [Test]
        public async Task EditArticleAsync_NullViewModelInput_ReturnsFalse()
        {
            var articleWasEdited = await _sut.EditArticleAsync(null, "test");

            Assert.That(!articleWasEdited);
        }
    }
}
