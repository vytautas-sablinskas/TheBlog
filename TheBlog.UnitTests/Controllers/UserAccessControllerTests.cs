using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TheBlog.Data.Utilities;
using TheBlog.MVC.Controllers;
using TheBlog.MVC.Services;
using TheBlog.MVC.ViewModels.Authentication;

namespace TheBlog.UnitTests.Controllers
{
    [TestFixture]
    public class UserAccessControllerTests
    {
        private Mock<IUserAccessService> _mockUserService;
        private Mock<IRoleService> _mockRoleService;
        private UserAccessController _controller;

        private readonly RegisterViewModel _registerViewModel = new();
        private readonly LoginViewModel _loginViewModel = new(username: "fake", password: "fake");
        private readonly ForgotPasswordViewModel _forgotPasswordViewModel = new();
        private readonly ResetPasswordViewModel _resetPasswordViewModel = new();

        private readonly TokenEmailValidityViewModel _tokenEmailValidityViewModel = new()
        {
            Token = "someToken",
            Email = "test@example.com"
        };

        [SetUp]
        public void SetUp()
        {
            _mockUserService = new Mock<IUserAccessService>();
            _mockRoleService = new Mock<IRoleService>();
            _controller = new UserAccessController(_mockUserService.Object, _mockRoleService.Object);

            _controller.ModelState.Clear();
        }

        [Test]
        public async Task Register_WithInvalidModelState_ReturnsJsonWithErrors()
        {
            _controller.ModelState.AddModelError("test", "test error");

            var result = await _controller.Register(_registerViewModel) as JsonResult;

            Assert.That((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value), Is.False);
        }

        [Test]
        public async Task Register_WithValidModelStateAndFailedServiceCall_ReturnsJsonWithError()
        {
            _mockUserService.Setup(m => m.AddAsync(_registerViewModel))
                .ReturnsAsync(IdentityResult.Failed());

            var result = await _controller.Register(_registerViewModel) as JsonResult;

            Assert.That((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value), Is.False);
        }

        [Test]
        public async Task Register_WithValidModelStateAndSuccessfulServiceCall_ReturnsSuccessJson()
        {
            _mockUserService.Setup(m => m.AddAsync(_registerViewModel))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserService.Setup(m => m.LoginAsync(It.IsAny<LoginViewModel>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _mockRoleService.Setup(r => r.GetUserRolesByUsernameAsync(_registerViewModel.Username))
                .ReturnsAsync(AppRoles.Roles);

            var result = await _controller.Register(_registerViewModel) as JsonResult;

            Assert.That((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value), Is.True);
            Assert.That((List<string>)result.Value.GetType().GetProperty("userRoles").GetValue(result.Value), Is.EqualTo(AppRoles.Roles));
        }

        [Test]
        public async Task Login_ServiceCallReturnsFailure_ReturnsFailureJson()
        {
            _mockUserService.Setup(m => m.LoginAsync(_loginViewModel))
                            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var result = await _controller.Login(_loginViewModel) as JsonResult;

            Assert.That((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value), Is.False);
        }

        [Test]
        public async Task Login_ServiceCallReturnsSuccess_ReturnsSuccessJson()
        {
            _mockUserService.Setup(m => m.LoginAsync(_loginViewModel))
                            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var result = await _controller.Login(_loginViewModel) as JsonResult;

            Assert.That((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value), Is.True);
        }

        [Test]
        public async Task ForgotPassword_ServiceReturnsInformation_ReturnsSameInformationInJson()
        {
            var expectedSuccess = true;
            var expectedMessage = "test message";
            _mockUserService.Setup(m => m.ForgotPasswordAsync(_forgotPasswordViewModel))
                            .ReturnsAsync((success: expectedSuccess, message: expectedMessage));

            var result = await _controller.ForgotPassword(_forgotPasswordViewModel) as JsonResult;
            Assert.Multiple(() =>
            {
                Assert.That((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value), Is.EqualTo(expectedSuccess));
                Assert.That(result.Value.GetType().GetProperty("message").GetValue(result.Value), Is.EqualTo(expectedMessage));
            });
        }

        [Test]
        public async Task ResetPassword_Get_ShouldReturnValidJsonResult()
        {
            _mockUserService.Setup(x => x.IsTokenAndEmailValidAsync(_tokenEmailValidityViewModel))
                            .ReturnsAsync(true);

            var result = await _controller.ResetPassword(_tokenEmailValidityViewModel) as JsonResult;

            Assert.That(result.Value.GetType().GetProperty("isTokenAndEmailValid").GetValue(result.Value), Is.EqualTo(true));
        }

        [Test]
        public async Task ResetPassword_PostValidInput_ShouldReturnSuccessJsonResult()
        {
            var expectedSuccess = true;
            var expectedMessage = "Password was successfully reset.";
            _mockUserService.Setup(x => x.ResetPasswordAsync(_resetPasswordViewModel))
                            .ReturnsAsync(IdentityResult.Success);

            var result = await _controller.ResetPassword(_resetPasswordViewModel) as JsonResult;

            Assert.Multiple(() =>
            {
                Assert.That((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value), Is.EqualTo(expectedSuccess));
                Assert.That(result.Value.GetType().GetProperty("message").GetValue(result.Value), Is.EqualTo(expectedMessage));
            });
        }

        [Test]
        public async Task ResetPassword_PostInvalidInput_ShouldReturnFailureJsonResult()
        {
            var expectedSuccess = false;
            var expectedMessage = "Failed to reset password. Try again later";
            _mockUserService.Setup(x => x.ResetPasswordAsync(_resetPasswordViewModel))
                            .ReturnsAsync(IdentityResult.Failed());

            var result = await _controller.ResetPassword(_resetPasswordViewModel) as JsonResult;

            Assert.Multiple(() =>
            {
                Assert.That((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value), Is.EqualTo(expectedSuccess));
                Assert.That(result.Value.GetType().GetProperty("message").GetValue(result.Value), Is.EqualTo(expectedMessage));
            });
        }
    }
}