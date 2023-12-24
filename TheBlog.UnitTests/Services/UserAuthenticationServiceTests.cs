using Microsoft.AspNetCore.Identity;
using Moq;
using TheBlog.API.Dtos.Authentication;
using TheBlog.API.Services;
using TheBlog.Data.Entities;
using TheBlog.Data.Utilities;
using TheBlog.MVC.Wrappers;

namespace TheBlog.UnitTests.Services
{
    public class UserAuthenticationServiceTests
    {
        private Mock<IUserManagerWrapper<User>> _mockUserManagerWrapper;
        private Mock<IJwtTokenService> _mockJwtTokenService;

        private UserAuthenticationService _sut;

        [SetUp]
        public void SetUp()
        {
            _mockUserManagerWrapper = new Mock<IUserManagerWrapper<User>>();
            _mockJwtTokenService = new Mock<IJwtTokenService>();

            _sut = new UserAuthenticationService(_mockUserManagerWrapper.Object, _mockJwtTokenService.Object);
        }

        [Test]
        public async Task LoginAsync_UserNotFound_ReturnsFailure()
        {
            _mockUserManagerWrapper.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                                   .ReturnsAsync((User)null);

            var result = await _sut.LoginAsync(new LoginDto("", ""));

            Assert.That(!result.Success);
            Assert.That(result.Message, Is.EqualTo("Username or password is invalid"));
        }

        [Test]
        public async Task LoginAsync_InvalidPassword_ReturnsFailure()
        {
            _mockUserManagerWrapper.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                                   .ReturnsAsync(new User());
            _mockUserManagerWrapper.Setup(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                                   .ReturnsAsync(false);

            var result = await _sut.LoginAsync(new LoginDto("", ""));

            Assert.That(!result.Success);
            Assert.That(result.Message, Is.EqualTo("Username or password is invalid"));
        }

        [Test]
        public async Task LoginAsync_ValidInputs_ReturnsSuccessWithLoginInformation()
        {
            _mockUserManagerWrapper.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                                   .ReturnsAsync(new User());
            _mockUserManagerWrapper.Setup(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                                   .ReturnsAsync(true);
            _mockUserManagerWrapper.Setup(m => m.GetRolesAsync(It.IsAny<User>()))
                                   .ReturnsAsync(new List<string> { AppRoleNames.Admin, AppRoleNames.ArticleWriter });
            _mockJwtTokenService.Setup(m => m.CreateTokens(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                                .Returns(("accessToken", "refreshToken"));

            var result = await _sut.LoginAsync(new LoginDto("", ""));

            Assert.That(result.Success);
            Assert.That(result.Value.AccessToken, Is.EqualTo("accessToken"));
            Assert.That(result.Value.RefreshToken, Is.EqualTo("refreshToken"));
        }

        [Test]
        public async Task RegisterAsync_UserAlreadyRegistered_ReturnsFailure()
        {
            _mockUserManagerWrapper.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                                   .ReturnsAsync(new User());

            var result = await _sut.RegisterAsync(new RegisterUserDto("", "", ""));

            Assert.That(!result.Success);
            Assert.That(result.Message, Is.EqualTo("Username is already taken!"));
        }

        [Test]
        public async Task RegisterAsync_FailureToCreate_ReturnsFailure()
        {
            _mockUserManagerWrapper.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                                   .ReturnsAsync((User)null);
            _mockUserManagerWrapper.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                                   .ReturnsAsync(IdentityResult.Failed());

            var result = await _sut.RegisterAsync(new RegisterUserDto("", "", ""));

            Assert.That(!result.Success);
            Assert.That(result.Message, Is.EqualTo("Failed creating user. Try again later!"));
        }

        [Test]
        public async Task RegisterAsync_ValidInputsAndCreatedInDatabase_ReturnsSuccess()
        {
            _mockUserManagerWrapper.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                                   .ReturnsAsync((User)null);
            _mockUserManagerWrapper.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                                   .ReturnsAsync(IdentityResult.Success);

            var result = await _sut.RegisterAsync(new RegisterUserDto("test", "test@gmail.com", ""));

            Assert.That(result.Success);
            Assert.That(result.Value.UserName, Is.EqualTo("test"));
            Assert.That(result.Value.Email, Is.EqualTo("test@gmail.com"));
        }

        [Test]
        public void Logout_RefreshTokenInvalid_ReturnsFailure()
        {
            _mockJwtTokenService.Setup(m => m.RevokeToken(It.IsAny<string>()))
                                .Returns(false);

            var result = _sut.Logout(new RefreshTokenDto(""));

            Assert.That(!result.Success);
            Assert.That(result.Message, Is.EqualTo("Invalid token!"));
        }

        [Test]
        public void Logout_TokenDtoIsValid_ReturnsSuccess()
        {
            _mockJwtTokenService.Setup(m => m.RevokeToken(It.IsAny<string>()))
                                .Returns(true);

            var result = _sut.Logout(new RefreshTokenDto(""));

            Assert.That(result.Success);
            Assert.That(result.Message, Is.EqualTo("User successfully logged out!"));
        }

        [Test]
        public async Task RefreshTokensAsync_InvalidInput_ReturnsFailure()
        {
            _mockJwtTokenService.Setup(m => m.RefreshTokensAsync(It.IsAny<IUserManagerWrapper<User>>(), It.IsAny<string>()))
                                             .ReturnsAsync(default((string AccessToken, string RefreshToken)?));

            var result = await _sut.RefreshTokensAsync(new RefreshTokenDto(""));

            Assert.That(!result.Success);
            Assert.That(result.Message, Is.EqualTo("Invalid or expired refresh token was given!"));
        }

        [Test]
        public async Task RefreshTokensAsync_ValidInput_ReturnsSuccess()
        {
            _mockJwtTokenService.Setup(m => m.RefreshTokensAsync(It.IsAny<IUserManagerWrapper<User>>(), It.IsAny<string>()))
                                             .ReturnsAsync(("newAccessToken", "newRefreshToken"));

            var result = await _sut.RefreshTokensAsync(new RefreshTokenDto(""));

            Assert.That(result.Success);
            Assert.That(result.Value.AccessToken, Is.EqualTo("newAccessToken"));
            Assert.That(result.Value.RefreshToken, Is.EqualTo("newRefreshToken"));
        }
    }
}
