using Microsoft.AspNetCore.Identity;
using Moq;
using TheBlog.Data.Utilities;
using TheBlog.MVC.Services;
using TheBlog.MVC.Wrappers;

namespace TheBlog.UnitTests.Services
{
    [TestFixture]
    public class RoleSeederServiceTests
    {
        private Mock<IRoleManagerWrapper> _mockRoleManagerWrapper;
        private RoleSeederService _sut;

        [SetUp]
        public void SetUp()
        {
            _mockRoleManagerWrapper = new Mock<IRoleManagerWrapper>();
            _sut = new RoleSeederService(_mockRoleManagerWrapper.Object);
        }

        [Test]
        public async Task SeedRolesAsync_OneRoleDoesNotExists_AddsOnlyOtherRoles()
        {
            var expectedCreateAsyncTimesCalled = AppRoles.Roles.Count - 1;
            _mockRoleManagerWrapper.Setup(m => m.RoleExistsAsync(AppRoles.Roles[0]))
                .ReturnsAsync(false);

            for (int i = 1; i < AppRoles.Roles.Count; i++)
            {
                _mockRoleManagerWrapper.Setup(m => m.RoleExistsAsync(AppRoles.Roles[i]))
                    .ReturnsAsync(true);
            }

            await _sut.SeedRolesAsync();

            _mockRoleManagerWrapper.Verify(m => m.CreateAsync(It.IsAny<IdentityRole>()), Times.Exactly(1));
        }
    }
}