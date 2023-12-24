using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using TheBlog.Data.Database;
using TheBlog.Data.Entities;
using TheBlog.Data.Utilities;
using TheBlog.MVC.Services;
using TheBlog.MVC.ViewModels.Profile;
using TheBlog.MVC.Wrappers;

namespace TheBlog.UnitTests.Services
{
    [TestFixture]
    public class UserProfileServiceTests
    {
        private Mock<IMapper> _mockMapper;
        private Mock<ISignInManagerWrapper<User>> _mockSignInManagerWrapper;
        private Mock<IUserManagerWrapper<User>> _mockUserManagerWrapper;
        private Mock<IRoleService> _mockRoleService;
        private Mock<IRepository<User>> _mockUserRepository;
        private UserProfileService _sut;

        private const string currentUsername = "existingUser";
        private const string newUsername = "takenUsername";

        private readonly User _user = new() { UserName = currentUsername };
        private readonly UserProfileViewModel _userProfileViewModel = new() { UserName = currentUsername };

        private readonly UserNewProfileViewModel _newProfileViewModel = new()
        {
            UserName = newUsername
        };

        private readonly UserNewPasswordViewModel _userNewPasswordViewModel = new()
        {
            CurrentPassword = "test",
            NewPassword = "test2",
            ConfirmPassword = "test2"
        };

        [SetUp]
        public void SetUp()
        {
            _mockMapper = new Mock<IMapper>();
            _mockSignInManagerWrapper = new Mock<ISignInManagerWrapper<User>>();
            _mockUserManagerWrapper = new Mock<IUserManagerWrapper<User>>();
            _mockRoleService = new Mock<IRoleService>();
            _mockUserRepository = new Mock<IRepository<User>>();

            _sut = new UserProfileService(_mockMapper.Object, _mockSignInManagerWrapper.Object, _mockUserManagerWrapper.Object, _mockRoleService.Object, _mockUserRepository.Object);
        }

        [Test]
        public async Task LogoutAsync_ShouldCallLogoutMethod()
        {
            await _sut.LogoutAsync();

            _mockSignInManagerWrapper.Verify(m => m.SignOutAsync(), Times.Once);
        }

        [Test]
        public async Task GetUserProfileViewModelAsync_MapsAndReturnsUserViewModel()
        {
            _mockUserManagerWrapper.Setup(m => m.FindByNameAsync(currentUsername))
                                   .ReturnsAsync(_user);
            _mockMapper.Setup(m => m.Map<UserProfileViewModel>(_user))
                       .Returns(_userProfileViewModel);

            var resultViewModel = await _sut.GetUserProfileViewModelAsync(currentUsername);

            Assert.That(resultViewModel, Is.Not.Null);
            Assert.That(resultViewModel, Is.EqualTo(_userProfileViewModel));
        }

        [Test]
        public async Task UpdateProfileAsync_UserDoesntExist_ReturnsFailed()
        {
            _mockUserManagerWrapper.Setup(x => x.FindByNameAsync(currentUsername))
                .ReturnsAsync((User)null);
            _mockUserManagerWrapper.Setup(x => x.FindByNameAsync(newUsername))
                .ReturnsAsync((User)null);

            var result = await _sut.UpdateProfileAsync(currentUsername, _newProfileViewModel);

            Assert.That(!result.Succeeded);
        }

        [Test]
        public async Task UpdateProfileAsync_UserExists_NewUsernameTaken_ReturnsFailed()
        {
            _mockUserManagerWrapper.Setup(x => x.FindByNameAsync(currentUsername))
                .ReturnsAsync(new User());
            _mockUserManagerWrapper.Setup(x => x.FindByNameAsync(newUsername))
                .ReturnsAsync(new User());

            var result = await _sut.UpdateProfileAsync(currentUsername, _newProfileViewModel);

            Assert.That(!result.Succeeded);
        }

        [Test]
        public async Task UpdateProfileAsync_ValidInputs_UpdatesProfileReloginsAndReturnsSuccess()
        {
            _mockUserManagerWrapper.Setup(x => x.FindByNameAsync(currentUsername))
                .ReturnsAsync(new User());
            _mockUserManagerWrapper.Setup(x => x.FindByNameAsync(newUsername))
                .ReturnsAsync((User)null);
            _mockUserManagerWrapper.Setup(m => m.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _sut.UpdateProfileAsync(currentUsername, _newProfileViewModel);

            _mockUserManagerWrapper.Verify(m => m.FindByNameAsync(It.IsAny<string>()), Times.Exactly(2));
            _mockUserManagerWrapper.Verify(m => m.UpdateAsync(It.IsAny<User>()), Times.Once);
            _mockMapper.Verify(m => m.Map(It.IsAny<UserNewProfileViewModel>(), It.IsAny<User>()), Times.Once);
            _mockSignInManagerWrapper.Verify(m => m.SignOutAsync(), Times.Once);
            _mockSignInManagerWrapper.Verify(m => m.SignInAsync(It.IsAny<User>(), It.IsAny<bool>(), null), Times.Once);
            Assert.That(result.Succeeded);
        }

        [Test]
        public async Task UpdatePasswordAsync_UserNotFound_ReturnsFail()
        {
            _mockUserManagerWrapper.Setup(m => m.FindByNameAsync(currentUsername))
                                   .ReturnsAsync((User)null);

            var result = await _sut.UpdatePasswordAsync(currentUsername, _userNewPasswordViewModel);

            Assert.That(!result.Succeeded);
            _mockUserManagerWrapper.Verify(m => m.ChangePasswordAsync(It.IsAny<User>(),
                                                                      _userNewPasswordViewModel.CurrentPassword,
                                                                      _userNewPasswordViewModel.NewPassword),
                                                                      Times.Never);
        }

        [Test]
        public async Task UpdatePasswordAsync_UserFound_ReturnsSuccess()
        {
            _mockUserManagerWrapper.Setup(m => m.FindByNameAsync(currentUsername))
                                   .ReturnsAsync(new User());
            _mockUserManagerWrapper.Setup(m => m.ChangePasswordAsync(It.IsAny<User>(),
                                                                     _userNewPasswordViewModel.CurrentPassword,
                                                                     _userNewPasswordViewModel.NewPassword))
                                                 .ReturnsAsync(IdentityResult.Success);

            var result = await _sut.UpdatePasswordAsync(currentUsername, _userNewPasswordViewModel);

            Assert.That(result.Succeeded);
            _mockUserManagerWrapper.Verify(m => m.FindByNameAsync(currentUsername), Times.Once);
            _mockUserManagerWrapper.Verify(m => m.ChangePasswordAsync(It.IsAny<User>(),
                                                                      _userNewPasswordViewModel.CurrentPassword,
                                                                      _userNewPasswordViewModel.NewPassword),
                                                                      Times.Once);
        }

        [Test]
        public void GetAllUserProfileViewModels()
        {
            var listOfUsers = new List<User>
            {
                new User(),
                new User()
            }.AsQueryable();

            _mockUserRepository.Setup(m => m.GetAll())
                .Returns(listOfUsers);

            var profiles = _sut.GetAllUserProfileViewModels();

            Assert.That(profiles, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetUserProfileWithRolesViewModelAsync_InvalidUser_ReturnsNull()
        {
            _mockUserManagerWrapper.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            var result = await _sut.GetUserProfileWithRolesViewModelAsync("fake");

            Assert.That(result, Is.Null);
            _mockMapper.Verify(m => m.Map<UserProfileWithRolesViewModel>(It.IsAny<UserProfileViewModel>()), Times.Never);
        }

        [Test]
        public async Task GetUserProfileWithRolesViewModelAsync_ValidUser_ReturnsUserProfileWithRolesViewModel()
        {
            _mockUserManagerWrapper.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            _mockMapper.Setup(m => m.Map<UserProfileWithRolesViewModel>(It.IsAny<UserProfileViewModel>()))
                .Returns(new UserProfileWithRolesViewModel());
            _mockRoleService.Setup(m => m.GetUserRolesByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(AppRoles.Roles);

            var result = await _sut.GetUserProfileWithRolesViewModelAsync("fakeUsername");

            Assert.That(result.Roles, Is.EqualTo(AppRoles.Roles));
            _mockMapper.Verify(m => m.Map<UserProfileWithRolesViewModel>(It.IsAny<UserProfileViewModel>()), Times.Once);
            _mockRoleService.Verify(m => m.GetUserRolesByUsernameAsync(It.IsAny<string>()), Times.Once);
        }
    }
}