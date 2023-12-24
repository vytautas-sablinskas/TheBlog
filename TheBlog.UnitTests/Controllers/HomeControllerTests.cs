using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TheBlog.MVC.Services;
using TheBlog.Server.Controllers;

namespace TheBlog.UnitTests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        private Mock<IRoleService> _mockRoleService;
        private Mock<IHomePageService> _mockHomePageService;
        private HomeController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRoleService = new Mock<IRoleService>();
            _mockHomePageService = new Mock<IHomePageService>();
            _controller = new HomeController(_mockRoleService.Object, _mockHomePageService.Object);
        }

        [Test]
        public async Task Index_UserIsAuthenticated_ShouldReturnUserRoles()
        {
            var expectedRoles = new List<string> { "Role1", "Role2" };
            _mockRoleService.Setup(service => service.GetUserRolesByUsernameAsync(It.IsAny<string>()))
                            .ReturnsAsync(expectedRoles);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testUser")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var result = await _controller.Index() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewData["UserRoles"], Is.EqualTo(expectedRoles));
        }

        [Test]
        public async Task Index_UserIsNotAuthenticated_ShouldReturnEmptyRoles()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var result = await _controller.Index() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewData["UserRoles"], Is.Empty);
        }
    }
}