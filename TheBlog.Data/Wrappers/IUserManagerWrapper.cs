using Microsoft.AspNetCore.Identity;

namespace TheBlog.MVC.Wrappers
{
    public interface IUserManagerWrapper<TUser>
    {
        Task<IdentityResult> CreateAsync(TUser user, string password);

        Task<TUser> FindByEmailAsync(string email);

        Task<TUser> FindByNameAsync(string userName);

        Task<TUser> FindByIdAsync(string userId);

        Task<string> GeneratePasswordResetTokenAsync(TUser user);

        Task<IdentityResult> ResetPasswordAsync(TUser user, string token, string newPassword);

        Task<bool> VerifyUserTokenAsync(TUser user, string tokenProvider, string purpose, string token);

        Task<IdentityResult> UpdateAsync(TUser user);

        Task<IdentityResult> ChangePasswordAsync(TUser user, string currentPassword, string newPassword);

        Task<bool> IsInRoleAsync(TUser user, string role);

        Task<IdentityResult> RemoveFromRoleAsync(TUser user, string role);

        Task<IdentityResult> AddToRoleAsync(TUser user, string role);

        Task<List<string>> GetRolesAsync(TUser user);

        Task<bool> CheckPasswordAsync(TUser user, string password);
    }
}