using Microsoft.AspNetCore.Identity;
using TheBlog.MVC.ViewModels.Profile;

namespace TheBlog.MVC.Services
{
    public interface IUserProfileService
    {
        Task LogoutAsync();

        Task<UserProfileViewModel> GetUserProfileViewModelAsync(string username);

        Task<IdentityResult> UpdateProfileAsync(string currentUsername, UserNewProfileViewModel newProfile);

        Task<IdentityResult> UpdatePasswordAsync(string currentUsername, UserNewPasswordViewModel viewModel);

        Task<UserProfileWithRolesViewModel> GetUserProfileWithRolesViewModelAsync(string username);

        List<UserProfileViewModel> GetAllUserProfileViewModels();
    }
}