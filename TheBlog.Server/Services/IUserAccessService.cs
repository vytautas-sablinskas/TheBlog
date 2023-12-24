using Microsoft.AspNetCore.Identity;
using TheBlog.MVC.ViewModels.Authentication;

namespace TheBlog.MVC.Services
{
    public interface IUserAccessService
    {
        Task<IdentityResult> AddAsync(RegisterViewModel viewModel);

        Task<SignInResult> LoginAsync(LoginViewModel viewModel);

        Task<(bool success, string message)> ForgotPasswordAsync(ForgotPasswordViewModel forgotPasswordViewModel);

        Task<bool> IsTokenAndEmailValidAsync(TokenEmailValidityViewModel viewModel);

        Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel viewModel);
    }
}