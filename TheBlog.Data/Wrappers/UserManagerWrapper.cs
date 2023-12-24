using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.MVC.Wrappers
{
    [ExcludeFromCodeCoverage]
    public class UserManagerWrapper<TUser> : IUserManagerWrapper<TUser> where TUser : class
    {
        private readonly UserManager<TUser> _userManager;

        public UserManagerWrapper(UserManager<TUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateAsync(TUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(TUser user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<TUser> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<TUser> FindByNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<TUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> VerifyUserTokenAsync(TUser user, string tokenProvider, string purpose, string token)
        {
            return await _userManager.VerifyUserTokenAsync(user, tokenProvider, purpose, token);
        }

        public async Task<IdentityResult> ResetPasswordAsync(TUser user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<IdentityResult> UpdateAsync(TUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(TUser user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<bool> IsInRoleAsync(TUser user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(TUser user, string role)
        {
            return await _userManager.RemoveFromRoleAsync(user, role);
        }

        public async Task<IdentityResult> AddToRoleAsync(TUser user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<List<string>> GetRolesAsync(TUser user)
        {
            return (await _userManager.GetRolesAsync(user)).ToList();
        }

        public async Task<bool> CheckPasswordAsync(TUser user, string password)
        {
            return (await _userManager.CheckPasswordAsync(user, password));
        }
    }
}