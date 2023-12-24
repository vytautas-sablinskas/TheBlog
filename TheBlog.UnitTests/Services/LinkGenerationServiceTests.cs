using Moq;
using System.Web;
using TheBlog.MVC.Services;
using TheBlog.MVC.Wrappers;

namespace TheBlog.UnitTests.Services
{
    [TestFixture]
    public class LinkGenerationServiceTests
    {
        private Mock<IHttpRequestUrlWrapper> _mockRequestUrlWrapper;
        private LinkGenerationService _sut;

        [SetUp]
        public void SetUp()
        {
            _mockRequestUrlWrapper = new Mock<IHttpRequestUrlWrapper>();
            _sut = new LinkGenerationService(_mockRequestUrlWrapper.Object);
        }

        [Test]
        public async Task GeneratePasswordResetLink_ShouldReturnCorrectLink()
        {
            var link = "link/";
            var token = "fewafegearg3@$5_a";
            var email = "test@gmail.com";
            var encodedToken = HttpUtility.UrlEncode(token);
            var encodedEmail = HttpUtility.UrlEncode(email);
            var expectedResetLink = $"{link}reset-password?token={encodedToken}&email={encodedEmail}";

            _mockRequestUrlWrapper.Setup(m => m.GetBaseUrl())
                                .Returns(link);

            var resultLink = await _sut.GeneratePasswordResetLink(token, email);

            Assert.That(expectedResetLink, Is.EqualTo(resultLink));
            _mockRequestUrlWrapper.Verify(m => m.GetBaseUrl(), Times.Once);
        }
    }
}