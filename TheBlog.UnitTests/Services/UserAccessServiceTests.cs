using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using TheBlog.Data.Entities;
using TheBlog.MVC.Services;
using TheBlog.MVC.ViewModels.Authentication;
using TheBlog.MVC.Wrappers;

namespace TheBlog.UnitTests.Services
{
    [TestFixture]
    public class UserAccessServiceTests
    {
        private Mock<IUserManagerWrapper<User>> _mockUserManagerWrapper;
        private Mock<ISignInManagerWrapper<User>> _mockSignInManagerWrapper;
        private Mock<IMapper> _mockMapper;
        private Mock<IEmailService> _mockEmailService;
        private UserAccessService _sut;

        private readonly RegisterViewModel _registerViewModel = new()
        {
            Username = "test",
            Email = "email@gmail.com",
            Password = "pw",
            ConfirmPassword = "pw"
        };

        private readonly LoginViewModel _loginViewModel = new(username: "test", password: "pw");

        private readonly User _user = new()
        {
            Email = "correct@gmail.com"
        };

        private readonly TokenEmailValidityViewModel _tokenAndEmailValidityViewModel = new()
        {
            Token = "faketoken",
            Email = "fakeemail@gmail.com"
        };

        private readonly ResetPasswordViewModel _resetPasswordViewModel = new()
        {
            Token = "fake token",
            Email = "fake@gmail.com",
            NewPassword = "newPassword",
            ConfirmPassword = "newPassword"
        };

        [SetUp]
        public void SetUp()
        {
            _mockUserManagerWrapper = new Mock<IUserManagerWrapper<User>>();
            _mockSignInManagerWrapper = new Mock<ISignInManagerWrapper<User>>();
            _mockMapper = new Mock<IMapper>();
            _mockEmailService = new Mock<IEmailService>();

            _sut = new UserAccessService(_mockUserManagerWrapper.Object,
                                         _mockSignInManagerWrapper.Object,
                                         _mockMapper.Object,
                                         _mockEmailService.Object);
        }

        [Test]
        public async Task AddAsync_CallsOtherServicesAndReturnsStatus()
        {
            _mockUserManagerWrapper.Setup(m => m.CreateAsync(It.IsAny<User>(), _registerViewModel.Password))
                                   .ReturnsAsync(IdentityResult.Success);

            var result = await _sut.AddAsync(_registerViewModel);

            _mockMapper.Verify(m => m.Map<User>(_registerViewModel), Times.Once);
            Assert.That(result.Succeeded);
        }

        [Test]
        public async Task LoginAsync_ShouldCallServicesAndReturnsStatus()
        {
            _mockSignInManagerWrapper.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(),
                                                                       It.IsAny<string>(),
                                                                       It.IsAny<bool>(),
                                                                       It.IsAny<bool>()))
                                     .ReturnsAsync(SignInResult.Failed);

            var result = await _sut.LoginAsync(_loginViewModel);

            _mockSignInManagerWrapper.Verify(m => m.PasswordSignInAsync(It.IsAny<string>(),
                                                                        It.IsAny<string>(),
                                                                        It.IsAny<bool>(),
                                                                        It.IsAny<bool>()),
                                                                        Times.Once);
            Assert.That(!result.Succeeded);
        }

        [Test]
        public async Task ForgotPasswordAsync_UserNotFound_ReturnsFailureStatusAndMessage()
        {
            var forgotPasswordViewModel = new ForgotPasswordViewModel
            {
                Username = "test",
                Email = "test@gmail.com"
            };
            var expectedSuccess = false;
            var expectedMessage = "Username or email is incorrect.";

            _mockUserManagerWrapper.Setup(m => m.FindByNameAsync(forgotPasswordViewModel.Username))
                                   .ReturnsAsync((User)null);

            (var success, var message) = await _sut.ForgotPasswordAsync(forgotPasswordViewModel);
            Assert.Multiple(() =>
            {
                Assert.That(success, Is.EqualTo(expectedSuccess));
                Assert.That(message, Is.EqualTo(expectedMessage));
            });
        }

        [Test]
        public async Task ForgotPasswordAsync_EmailGivenNotSameAsUserEmail_ReturnsFailureStatusAndMessage()
        {
            var forgotPasswordViewModel = new ForgotPasswordViewModel
            {
                Username = "test",
                Email = "incorrect@gmail.com"
            };
            var expectedSuccess = false;
            var expectedMessage = "Username or email is incorrect.";

            _mockUserManagerWrapper.Setup(m => m.FindByNameAsync(forgotPasswordViewModel.Username))
                                   .ReturnsAsync(_user);

            (var success, var message) = await _sut.ForgotPasswordAsync(forgotPasswordViewModel);

            Assert.Multiple(() =>
            {
                Assert.That(success, Is.EqualTo(expectedSuccess));
                Assert.That(message, Is.EqualTo(expectedMessage));
            });
        }

        [Test]
        public async Task ForgotPasswordAsync_ValidInput_ReturnsSuccessStatusAndMessage()
        {
            var forgotPasswordViewModel = new ForgotPasswordViewModel
            {
                Username = "test",
                Email = "correct@gmail.com"
            };

            var token = "fakeToken";
            var expectedSuccess = true;
            var expectedMessage = "Password reset link has been sent to your email.";

            _mockUserManagerWrapper.Setup(m => m.FindByNameAsync(forgotPasswordViewModel.Username))
                                   .ReturnsAsync(_user);
            _mockUserManagerWrapper.Setup(m => m.GeneratePasswordResetTokenAsync(_user))
                                   .ReturnsAsync(token);
            _mockEmailService.Setup(m => m.SendResetPasswordEmailAsync(token, _user.Email))
                             .ReturnsAsync((expectedSuccess, expectedMessage));

            (var success, var message) = await _sut.ForgotPasswordAsync(forgotPasswordViewModel);

            Assert.Multiple(() =>
            {
                Assert.That(success, Is.EqualTo(expectedSuccess));
                Assert.That(message, Is.EqualTo(expectedMessage));
            });
        }

        [Test]
        public async Task IsTokenAndEmailValidAsync_UserNotFound_ReturnsFalse()
        {
            _mockUserManagerWrapper.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                                   .ReturnsAsync((User)null);

            var tokenIsValid = await _sut.IsTokenAndEmailValidAsync(_tokenAndEmailValidityViewModel);

            Assert.That(!tokenIsValid);
        }

        [Test]
        public async Task IsTokenAndEmailValidAsync_ValidInput_ReturnsTrue()
        {
            _mockUserManagerWrapper.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                                   .ReturnsAsync(new User());
            _mockUserManagerWrapper.Setup(m => m.VerifyUserTokenAsync(It.IsAny<User>(),
                                                                      It.IsAny<string>(),
                                                                      It.IsAny<string>(),
                                                                      It.IsAny<string>()))
                                    .ReturnsAsync(true);

            var tokenIsValid = await _sut.IsTokenAndEmailValidAsync(_tokenAndEmailValidityViewModel);

            Assert.That(tokenIsValid);
        }

        [Test]
        public async Task ResetPasswordAsync_UserNotFound_ReturnsFailureResult()
        {
            _mockUserManagerWrapper.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                                   .ReturnsAsync((User)null);

            var result = await _sut.ResetPasswordAsync(_resetPasswordViewModel);

            Assert.That(!result.Succeeded);
        }

        [Test]
        public async Task ResetPasswordAsync_ValidInput_ReturnsResetPasswordResult()
        {
            _mockUserManagerWrapper.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                                   .ReturnsAsync(new User());
            _mockUserManagerWrapper.Setup(m => m.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                                   .ReturnsAsync(IdentityResult.Success);

            var result = await _sut.ResetPasswordAsync(_resetPasswordViewModel);

            Assert.That(result.Succeeded);
        }
    }
}