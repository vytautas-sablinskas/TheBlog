using Microsoft.IdentityModel.Tokens;
using Moq;
using System.Data;
using System.Linq.Expressions;
using TheBlog.API.Services;
using TheBlog.Data.Database;
using TheBlog.Data.Entities;
using TheBlog.MVC.Wrappers;

namespace TheBlog.UnitTests.Services
{
    public class JwtTokenServiceTests
    {
        private Mock<IRepository<RefreshToken>> _mockRefreshTokenRepository;
        private Mock<IUserManagerWrapper<User>> _mockUserManagerWrapper;

        private JwtTokenService _sut;

        [SetUp]
        public void SetUp()
        {
            _mockUserManagerWrapper = new Mock<IUserManagerWrapper<User>>();
            _mockRefreshTokenRepository = new Mock<IRepository<RefreshToken>>();
            _sut = new JwtTokenService(_mockRefreshTokenRepository.Object);
        }

        [Test]
        public void RevokeToken_TokenNotFoundInDatabase_ReturnsFalse()
        {
            _mockRefreshTokenRepository.Setup(m => m.FindByCondition(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                    .Returns(new List<RefreshToken>().AsQueryable());

            var tokenWasRevoked = _sut.RevokeToken("fakeToken");

            Assert.That(!tokenWasRevoked);
        }

        [Test]
        public void RevokeToken_TokenFound_ReturnsSuccess()
        {
            _mockRefreshTokenRepository.Setup(m => m.FindByCondition(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                    .Returns(new List<RefreshToken> { new RefreshToken { Token = "fakeToken "} }.AsQueryable());

            var tokenWasRevoked = _sut.RevokeToken("fakeToken");

            Assert.That(tokenWasRevoked);
        }

        [Test]
        public async Task RefreshTokensAsync_TokenNotFound_ReturnsNull()
        {
            _mockRefreshTokenRepository.Setup(m => m.FindByCondition(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                                       .Returns(new List<RefreshToken>().AsQueryable());

            var refreshedTokens = await _sut.RefreshTokensAsync(_mockUserManagerWrapper.Object, "fakeToken");

            Assert.That(refreshedTokens, Is.Null);
        }

        [Test]
        public async Task RefreshTokensAsync_TokenAlreadyExpired_ReturnsNull()
        {
            var refreshToken = new RefreshToken
            {
                Token = "fakeToken",
                ExpiryDate = DateTime.UtcNow.AddDays(-1)
            };

            _mockRefreshTokenRepository.Setup(m => m.FindByCondition(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                                       .Returns(new List<RefreshToken> { refreshToken }.AsQueryable());

            var refreshedTokens = await _sut.RefreshTokensAsync(_mockUserManagerWrapper.Object, "fakeToken");

            Assert.That(refreshedTokens, Is.Null);
        }

        [Test]
        public async Task RefreshTokensAsync_TokenDoesNotBelongToRequester_ReturnsNull()
        {
            var refreshToken = new RefreshToken
            {
                Token = "fakeToken",
                ExpiryDate = DateTime.UtcNow.AddDays(1)
            };

            _mockRefreshTokenRepository.Setup(m => m.FindByCondition(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                                       .Returns(new List<RefreshToken> { refreshToken }.AsQueryable());

            _mockUserManagerWrapper.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                                   .ReturnsAsync((User)null);

            var refreshedTokens = await _sut.RefreshTokensAsync(_mockUserManagerWrapper.Object, "fakeToken");

            Assert.That(refreshedTokens, Is.Null);
        }

        [Test]
        public async Task RefreshTokensAsync_ValidInputsAndRequester_ReturnsNewTokens()
        {
            var refreshToken = new RefreshToken
            {
                Token = "fakeToken",
                ExpiryDate = DateTime.UtcNow.AddDays(1)
            };

            _mockRefreshTokenRepository.Setup(m => m.FindByCondition(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                                       .Returns(new List<RefreshToken> { refreshToken }.AsQueryable());

            _mockUserManagerWrapper.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                                   .ReturnsAsync(new User { UserName = "test", Id = "id" } );
            _mockUserManagerWrapper.Setup(m => m.GetRolesAsync(It.IsAny<User>()))
                                   .ReturnsAsync(new List<string>());

            var refreshedTokens = await _sut.RefreshTokensAsync(_mockUserManagerWrapper.Object, "fakeToken");

            Assert.That(refreshedTokens, Is.Not.Null);
        }
    }
}
