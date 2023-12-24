using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.MVC.Wrappers
{
    [ExcludeFromCodeCoverage]
    public class SignInManagerWrapper<TUser> : ISignInManagerWrapper<TUser> where TUser : class
    {
        private readonly SignInManager<TUser> _signInManager;

        public SignInManagerWrapper(SignInManager<TUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task SignInAsync(TUser user, bool isPersistent, string? authenticationMethod = null)
        {
            await _signInManager.SignInAsync(user, isPersistent, authenticationMethod);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return await _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        }
    }
}