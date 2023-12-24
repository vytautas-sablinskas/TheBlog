using Microsoft.AspNetCore.Identity;
using TheBlog.Data.Entities;
using TheBlog.MVC.Wrappers;

namespace TheBlog.API.Services
{
    public interface IJwtTokenService
    {
        (string AccessToken, string RefreshToken) CreateTokens(string userName, string userId, IEnumerable<string> userRoles);
        Task<(string AccessToken, string RefreshToken)?> RefreshTokensAsync(IUserManagerWrapper<User> userManager, string refreshToken);
        bool RevokeToken(string refreshToken);
    }
}