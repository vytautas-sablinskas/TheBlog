using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheBlog.Data.Utilities;
using TheBlog.MVC.Services;
using TheBlog.MVC.ViewModels.Profile;

namespace TheBlog.MVC.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _userProfileService.LogoutAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        [Authorize(Roles = AppRoleNames.Admin)]
        public IActionResult UserProfiles()
        {
            var allUserProfiles = _userProfileService.GetAllUserProfileViewModels();

            return Json(new { allUserProfiles });
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var userProfile = await _userProfileService.GetUserProfileViewModelAsync(User.Identity.Name);

            return Json(new { userProfile });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] UserNewProfileViewModel viewModel)
        {
            var result = await _userProfileService.UpdateProfileAsync(User.Identity.Name, viewModel);
            if (!result.Succeeded)
            {
                return Json(new { success = false, errorMessage = "Username is already taken." });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword([FromBody] UserNewPasswordViewModel viewModel)
        {
            var result = await _userProfileService.UpdatePasswordAsync(User.Identity.Name, viewModel);
            if (!result.Succeeded)
            {
                return Json(new { success = false, errorMessage = "Your current password was incorrect." });
            }

            return Json(new { success = true });
        }

        [HttpGet]
        [Authorize(Roles = AppRoleNames.Admin)]
        public async Task<IActionResult> ProfileWithRoles([FromQuery] string username)
        {
            var userProfileWithRolesViewModel = await _userProfileService.GetUserProfileWithRolesViewModelAsync(username);

            return Json(new { userProfile = userProfileWithRolesViewModel });
        }
    }
}