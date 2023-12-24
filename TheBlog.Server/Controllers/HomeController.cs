using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using TheBlog.MVC.Services;

namespace TheBlog.Server.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IHomePageService _homePageService;

        public HomeController(IRoleService roleService, IHomePageService homePageService)
        {
            _roleService = roleService;
            _homePageService = homePageService;
        }

        public async Task<IActionResult> Index()
        {
            List<string> userRoles = new();
            if (User.Identity.IsAuthenticated)
            {
                userRoles = await _roleService.GetUserRolesByUsernameAsync(User.Identity.Name);
            }

            ViewData["UserRoles"] = userRoles;

            return View();
        }

        [HttpGet]
        public IActionResult Home()
        {
            var homePageViewModel = _homePageService.GetHomePageArticles();

            return Json(new { homePageViewModel });
        }
    }
}