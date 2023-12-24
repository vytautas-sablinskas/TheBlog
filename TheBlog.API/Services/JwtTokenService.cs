using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TheBlog.Data.Database;
using TheBlog.Data.Entities;
using TheBlog.MVC.Wrappers;

namespace TheBlog.API.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly SymmetricSecurityKey _authSigningKey;
        private readonly IRepository<RefreshToken> _refreshTokenRepository;

        public JwtTokenService(IRepository<RefreshToken> refreshTokenRepository)
        {
            _authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("ISSUER_KEY", EnvironmentVariableTarget.User)));
            _refreshTokenRepository = refreshTokenRepository;
        }

        public (string AccessToken, string RefreshToken) CreateTokens(string userName, string userId, IEnumerable<string> userRoles)
        {
            var accessToken = CreateAccessToken(userName, userId, userRoles);
            var refreshTokenString = GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                UserId = userId
            };

            var oldRefreshToken = _refreshTokenRepository.FindByCondition(r => r.UserId == userId)
                                                         .FirstOrDefault();
            if (oldRefreshToken != null)
            {
                _refreshTokenRepository.Delete(oldRefreshToken);
            }

            _refreshTokenRepository.Create(refreshToken);

            return (accessToken, refreshTokenString);
        }

        public async Task<(string AccessToken, string RefreshToken)?> RefreshTokensAsync(IUserManagerWrapper<User> userManager, string refreshToken)
        {
            var storedToken = _refreshTokenRepository.FindByCondition(r => r.Token == refreshToken)
                                                     .FirstOrDefault();
            if (storedToken == null || storedToken.ExpiryDate <= DateTime.UtcNow)
            {
                return null;
            }

            var user = await userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
            {
                return null;
            }

            var userRoles = await userManager.GetRolesAsync(user);
            var (newAccessToken, newRefreshToken) = CreateTokens(user.UserName, user.Id, userRoles);

            return (newAccessToken, newRefreshToken);
        }

        public bool RevokeToken(string refreshToken)
        {
            var storedToken = _refreshTokenRepository.FindByCondition(r => r.Token == refreshToken)
                                                     .FirstOrDefault();

            if (storedToken == null)
            {
                return false;
            }

            _refreshTokenRepository.Delete(storedToken);

            return true;
        }

        private string CreateAccessToken(string userName, string userId, IEnumerable<string> userRoles)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(JwtRegisteredClaimNames.Sub, userId)
            };

            authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

            var accessSecurityToken = new JwtSecurityToken
            (
                expires: DateTime.UtcNow.AddMinutes(120),
                claims: authClaims,
                signingCredentials: new SigningCredentials(_authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(accessSecurityToken);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }
    }
}