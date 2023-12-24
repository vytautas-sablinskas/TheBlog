using Microsoft.AspNetCore.Identity;

namespace TheBlog.MVC.Wrappers
{
    public interface ISignInManagerWrapper<TUser> where TUser : class
    {
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);
        Task SignInAsync(TUser user, bool isPersistent, string authenticationMethod = null);
        Task SignOutAsync();
    }
}