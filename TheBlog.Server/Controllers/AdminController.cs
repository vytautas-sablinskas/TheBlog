using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBlog.Data.Utilities;
using TheBlog.MVC.Services;
using TheBlog.MVC.ViewModels.Admin;

namespace TheBlog.MVC.Controllers
{
    [Authorize(Roles = AppRoleNames.Admin)]
    public class AdminController : Controller
    {
        private readonly IRoleService _roleService;

        public AdminController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<IActionResult> ChangeUserRoles([FromBody] ChangeUserRolesViewModel viewModel)
        {
            var hasAddedAllRoles = await _roleService.AddRolesToUserAsync(viewModel.UserName, viewModel.RolesToAdd);
            var hasRemovedAllRoles = await _roleService.RemoveRolesFromUserAsync(viewModel.UserName, viewModel.RolesToRemove);

            var success = hasAddedAllRoles && hasRemovedAllRoles;
            return Json(new { success });
        }
    }
}