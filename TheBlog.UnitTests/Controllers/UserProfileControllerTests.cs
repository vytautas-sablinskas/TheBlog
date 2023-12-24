using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TheBlog.MVC.Controllers;
using TheBlog.MVC.Services;
using TheBlog.MVC.ViewModels.Profile;

namespace TheBlog.UnitTests.Controllers
{
    [TestFixture]
    public class UserProfileControllerTests
    {
        private Mock<IUserProfileService> _mockUserProfileService;
        private UserProfileController _controller;

        private readonly UserProfileViewModel _userProfileViewModel = new();
        private readonly UserNewProfileViewModel _userNewProfileViewModel = new();
        private readonly UserNewPasswordViewModel _userNewPasswordViewModel = new();

        [SetUp]
        public void SetUp()
        {
            _mockUserProfileService = new Mock<IUserProfileService>();
            _controller = new UserProfileController(_mockUserProfileService.Object);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testUser")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthentication");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _controller.ModelState.Clear();
        }

        [Test]
        public async Task Logout_ShouldCallServiceAndReturnTrue()
        {
            var result = await _controller.Logout() as JsonResult;

            _mockUserProfileService.Verify(m => m.LogoutAsync(), Times.Once());
            Assert.That((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value), Is.True);
        }

        [Test]
        public async Task UserProfile_ShouldCallServiceAndReturnUserProfileViewModel()
        {
            _mockUserProfileService.Setup(m => m.GetUserProfileViewModelAsync(It.IsAny<string>()))
                                   .ReturnsAsync(_userProfileViewModel);

            var result = await _controller.UserProfile() as JsonResult;

            Assert.That((UserProfileViewModel)result.Value.GetType().GetProperty("userProfile").GetValue(result.Value), Is.EqualTo(_userProfileViewModel));
        }

        [Test]
        public async Task UpdateProfile_ServiceCallReturnsFailure_ReturnsFailureJson()
        {
            _mockUserProfileService.Setup(m => m.UpdateProfileAsync(It.IsAny<string>(), _userNewProfileViewModel))
                                   .ReturnsAsync(IdentityResult.Failed());

            var result = await _controller.UpdateProfile(_userNewProfileViewModel) as JsonResult;

            Assert.Multiple(() =>
            {
                Assert.That((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value), Is.False);
                Assert.That(result.Value.GetType().GetProperty("errorMessage").GetValue(result.Value), Is.EqualTo("Username is already taken."));
            });
        }

        [Test]
        public async Task UpdateProfile_ServiceCallReturnsSuccess_ReturnsSuccessJson()
        {
            _mockUserProfileService.Setup(m => m.UpdateProfileAsync(It.IsAny<string>(), _userNewProfileViewModel))
                                   .ReturnsAsync(IdentityResult.Success);

            var result = await _controller.UpdateProfile(_userNewProfileViewModel) as JsonResult;

            Assert.That((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value), Is.True);
        }

        [Test]
        public async Task UpdatePassword_ServiceCallReturnsFailure_ReturnsFailureJson()
        {
            _mockUserProfileService.Setup(m => m.UpdatePasswordAsync(It.IsAny<string>(), _userNewPasswordViewModel))
                                   .ReturnsAsync(IdentityResult.Failed());

            var result = await _controller.UpdatePassword(_userNewPasswordViewModel) as JsonResult;

            Assert.Multiple(() =>
            {
                Assert.That((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value), Is.False);
                Assert.That(result.Value.GetType().GetProperty("errorMessage").GetValue(result.Value), Is.EqualTo("Your current password was incorrect."));
            });
        }

        [Test]
        public async Task UpdatePassword_ServiceCallReturnsSuccess_ReturnsSuccessJson()
        {
            _mockUserProfileService.Setup(m => m.UpdatePasswordAsync(It.IsAny<string>(), _userNewPasswordViewModel))
                                   .ReturnsAsync(IdentityResult.Success);

            var result = await _controller.UpdatePassword(_userNewPasswordViewModel) as JsonResult;

            Assert.That((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value), Is.True);
        }
    }
}