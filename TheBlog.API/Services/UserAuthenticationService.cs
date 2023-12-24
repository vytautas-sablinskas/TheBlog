using TheBlog.API.Dtos.Authentication;
using TheBlog.Data.Entities;
using TheBlog.Data.Utilities;
using TheBlog.MVC.Wrappers;

namespace TheBlog.API.Services
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly IUserManagerWrapper<User> _userManagerWrapper;
        private readonly IJwtTokenService _jwtTokenService;

        public UserAuthenticationService(IUserManagerWrapper<User> userManagerWrapper, IJwtTokenService jwtTokenService)
        {
            _userManagerWrapper = userManagerWrapper;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<ResultWithData<SuccessfulLoginDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManagerWrapper.FindByNameAsync(loginDto.Username);
            if (user == null)
            {
                return new ResultWithData<SuccessfulLoginDto>
                {
                    Success = false,
                    Message = "Username or password is invalid"
                };
            }

            var passwordIsValid = await _userManagerWrapper.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordIsValid)
            {
                return new ResultWithData<SuccessfulLoginDto>
                {
                    Success = false,
                    Message = "Username or password is invalid"
                };
            }

            var roles = await _userManagerWrapper.GetRolesAsync(user);
            var (accessToken, refreshToken) = _jwtTokenService.CreateTokens(user.UserName, user.Id, roles);

            return new ResultWithData<SuccessfulLoginDto>
            {
                Success = true,
                Value = new SuccessfulLoginDto(accessToken, refreshToken)
            };
        }

        public async Task<ResultWithData<UserDto>> RegisterAsync(RegisterUserDto registerUserDto)
        {
            var user = await _userManagerWrapper.FindByNameAsync(registerUserDto.Username);
            if (user != null)
            {
                return new ResultWithData<UserDto>
                {
                    Success = false,
                    Message = "Username is already taken!"
                };
            }

            var newUser = new User
            {
                UserName = registerUserDto.Username,
                Email = registerUserDto.Email,
            };

            var creationResult = await _userManagerWrapper.CreateAsync(newUser, registerUserDto.Password);
            if (!creationResult.Succeeded)
            {
                return new ResultWithData<UserDto>
                {
                    Success = false,
                    Message = "Failed creating user. Try again later!"
                };
            }

            return new ResultWithData<UserDto>
            {
                Success = true,
                Value = new UserDto
                (
                    Id: newUser.Id,
                    UserName: newUser.UserName,
                    Email: newUser.Email
                )
            };
        }

        public Result Logout(RefreshTokenDto refreshTokenDto)
        {
            var tokenWasRevoked = _jwtTokenService.RevokeToken(refreshTokenDto.RefreshToken);
            if (!tokenWasRevoked)
            {
                return new Result
                {
                    Success = false,
                    Message = "Invalid token!"
                };
            }

            return new Result
            {
                Success = true,
                Message = "User successfully logged out!"
            };
        }

        public async Task<ResultWithData<SuccessfulLoginDto>> RefreshTokensAsync(RefreshTokenDto refreshTokenDto)
        {
            var newTokens = await _jwtTokenService.RefreshTokensAsync(_userManagerWrapper, refreshTokenDto.RefreshToken);
            if (newTokens == null)
            {
                return new ResultWithData<SuccessfulLoginDto>
                {
                    Success = false,
                    Message = "Invalid or expired refresh token was given!"
                };
            }

            return new ResultWithData<SuccessfulLoginDto>
            {
                Success = true,
                Value = new SuccessfulLoginDto
                (
                    AccessToken: newTokens.Value.AccessToken,
                    RefreshToken: newTokens.Value.RefreshToken
                )
            };
        }
    }
}