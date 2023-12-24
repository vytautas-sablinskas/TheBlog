using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Web;
using TheBlog.Data.Entities;
using TheBlog.MVC.ViewModels.Authentication;
using TheBlog.MVC.Wrappers;

namespace TheBlog.MVC.Services
{
    public class UserAccessService : IUserAccessService
    {
        private readonly IUserManagerWrapper<User> _userManagerWrapper;
        private readonly ISignInManagerWrapper<User> _signInManagerWrapper;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public UserAccessService(IUserManagerWrapper<User> userManagerWrapper, ISignInManagerWrapper<User> signInManagerWrapper, IMapper mapper, IEmailService emailService)
        {
            _userManagerWrapper = userManagerWrapper;
            _signInManagerWrapper = signInManagerWrapper;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<IdentityResult> AddAsync(RegisterViewModel viewModel)
        {
            var user = _mapper.Map<User>(viewModel);

            return await _userManagerWrapper.CreateAsync(user, viewModel.Password);
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel viewModel)
        {
            return await _signInManagerWrapper.PasswordSignInAsync(viewModel.Username.ToUpper(),
                                                            viewModel.Password,
                                                            isPersistent: true,
                                                            lockoutOnFailure: false);
        }

        public async Task<(bool success, string message)> ForgotPasswordAsync(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            var user = await _userManagerWrapper.FindByNameAsync(forgotPasswordViewModel.Username);
            if (user == null || !user.Email.Equals(forgotPasswordViewModel.Email, StringComparison.OrdinalIgnoreCase))
            {
                return (success: false, message: "Username or email is incorrect.");
            }

            var token = await _userManagerWrapper.GeneratePasswordResetTokenAsync(user);
            return await _emailService.SendResetPasswordEmailAsync(token, forgotPasswordViewModel.Email);
        }

        public async Task<bool> IsTokenAndEmailValidAsync(TokenEmailValidityViewModel viewModel)
        {
            var decodedEmail = HttpUtility.UrlDecode(viewModel.Email);
            var user = await _userManagerWrapper.FindByEmailAsync(decodedEmail);
            if (user == null)
            {
                return false;
            }

            return await _userManagerWrapper.VerifyUserTokenAsync(user,
                TokenOptions.DefaultProvider,
                UserManager<IdentityUser>.ResetPasswordTokenPurpose,
                viewModel.Token);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel viewModel)
        {
            var decodedEmail = HttpUtility.UrlDecode(viewModel.Email);
            var decodedToken = HttpUtility.UrlDecode(viewModel.Token);

            var user = await _userManagerWrapper.FindByEmailAsync(decodedEmail);
            if (user == null)
            {
                return IdentityResult.Failed();
            }

            return await _userManagerWrapper.ResetPasswordAsync(user, decodedToken, viewModel.NewPassword);
        }
    }
}