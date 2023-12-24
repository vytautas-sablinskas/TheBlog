using Microsoft.AspNetCore.Mvc;
using TheBlog.MVC.Services;
using TheBlog.MVC.ViewModels.Authentication;

namespace TheBlog.MVC.Controllers
{
    public class UserAccessController : Controller
    {
        private readonly IUserAccessService _userService;
        private readonly IRoleService _roleService;

        public UserAccessController(IUserAccessService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, errors = errorList });
            }

            var result = await _userService.AddAsync(model);

            if (!result.Succeeded)
            {
                return Json(new { success = false, errors = new List<string> { "An error occurred while processing your request. Try again later!" } });
            }

            var loginResult = await _userService.LoginAsync(new LoginViewModel(username: model.Username, password: model.Password));
            var userRoles = await _roleService.GetUserRolesByUsernameAsync(model.Username);

            return Json(new { success = loginResult.Succeeded, userRoles });
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel viewModel)
        {
            var result = await _userService.LoginAsync(viewModel);
            if (!result.Succeeded)
            {
                return Json(new { success = false, errorMessage = "Invalid username or password was provided." });
            }

            var userRoles = await _roleService.GetUserRolesByUsernameAsync(viewModel.Username);

            return Json(new { success = true, userRoles });
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel forgotPasswordViewModel)
        {
            var (success, message) = await _userService.ForgotPasswordAsync(forgotPasswordViewModel);

            return Json(new { success, message });
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword([FromQuery] TokenEmailValidityViewModel tokenEmailValidityViewModel)
        {
            var isTokenAndEmailValid = await _userService.IsTokenAndEmailValidAsync(tokenEmailValidityViewModel);

            return Json(new { isTokenAndEmailValid });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel resetPasswordViewModel)
        {
            var result = await _userService.ResetPasswordAsync(resetPasswordViewModel);
            if (!result.Succeeded)
            {
                return Json(new { success = false, message = "Failed to reset password. Try again later" });
            }

            return Json(new { success = true, message = "Password was successfully reset." });
        }
    }
}