using Moq;
using TheBlog.Data.Database;
using TheBlog.Data.Entities;
using TheBlog.MVC.Services;

namespace TheBlog.UnitTests.Services
{
    [TestFixture]
    public class ArticleFilteringServiceTests
    {
        private Mock<IRepository<Article>> _mockArticleRepository;

        private ArticleFilteringService _sut;

        [SetUp]
        public void SetUp()
        {
            _mockArticleRepository = new Mock<IRepository<Article>>();
            _sut = new ArticleFilteringService(_mockArticleRepository.Object);
        }

        [Test]
        public void GetArticles_WithTitleFilter_ReturnsFilteredArticles()
        {
            var articles = new List<Article>
            {
                new Article { Title = "Test Article 1" },
                new Article { Title = "Another Test Article" }
            };
            _mockArticleRepository.Setup(repo => repo.GetAll()).Returns(articles.AsQueryable());

            var result = _sut.GetArticles("Test Article 1");

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Test Article 1"));
        }

        [Test]
        public void GetArticles_WithEmptyTitleFilter_ReturnsAllArticles()
        {
            var articles = new List<Article>
            {
                new Article { Title = "Test Article 1" },
                new Article { Title = "Another Test Article" }
            };
            _mockArticleRepository.Setup(repo => repo.GetAll()).Returns(articles.AsQueryable());

            var result = _sut.GetArticles("");

            Assert.That(result, Has.Count.EqualTo(articles.Count));
        }

        [Test]
        public void GetLastFiveArticlesCreated_ReturnsLastFiveArticlesWithUsers()
        {
            var articles = GenerateMockArticles(10);
            var expectedOrder = articles.OrderByDescending(a => a.CreatedTime).Take(5).ToList();
            _mockArticleRepository.Setup(repo => repo.GetAll()).Returns(articles.AsQueryable());

            var result = _sut.GetLastFiveArticlesCreated();

            Assert.That(result, Has.Count.EqualTo(5));
            CollectionAssert.AreEqual(expectedOrder, result);
            Assert.That(result.All(a => a.User != null));
        }

        [Test]
        public void GetLastThreeArticlesByRatingDescending_IncludesUserInArticles()
        {
            var articles = GenerateMockArticles(10);
            var expectedOrder = articles.OrderByDescending(a => a.Rating).Take(3).ToList();
            _mockArticleRepository.Setup(repo => repo.GetAll()).Returns(articles.AsQueryable());

            var result = _sut.GetLastThreeArticlesByRatingDescending();

            Assert.That(result.All(a => a.User != null));
            CollectionAssert.AreEqual(expectedOrder, result);
            Assert.That(result, Has.Count.EqualTo(3));
        }

        [Test]
        public void GetLastThreeCommentedArticles_IncludesUserInArticles()
        {
            var articles = GenerateMockArticles(10);
            var expectedArticles = articles.Where(a => a.Comments.Any())
                                      .OrderByDescending(a => a.Comments.Max(c => c.Id))
                                      .Take(3)
                                      .ToList();

            _mockArticleRepository.Setup(repo => repo.GetAll()).Returns(articles.AsQueryable());

            var result = _sut.GetLastThreeCommentedArticles();

            Assert.That(result.All(a => a.User != null));
            CollectionAssert.AreEqual(expectedArticles, result);
            Assert.That(result, Has.Count.EqualTo(3));
        }

        private List<Article> GenerateMockArticles(int count)
        {
            var articles = new List<Article>();
            for (int i = 0; i < count; i++)
            {
                var article = new Article
                {
                    CreatedTime = DateTime.Now.AddHours(-i),
                    Rating = i,
                    User = new User(),
                    Comments = new List<Comment>()
                };

                for (int j = 0; j < 5; j++)
                {
                    article.Comments.Add(new Comment { Id = i * 10 + j });
                }

                articles.Add(article);
            }
            return articles;
        }
    }
}