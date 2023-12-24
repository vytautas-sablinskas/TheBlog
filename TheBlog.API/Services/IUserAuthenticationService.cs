using TheBlog.API.Dtos.Authentication;
using TheBlog.Data.Utilities;

namespace TheBlog.API.Services
{
    public interface IUserAuthenticationService
    {
        Task<ResultWithData<SuccessfulLoginDto>> LoginAsync(LoginDto loginDto);

        Task<ResultWithData<UserDto>> RegisterAsync(RegisterUserDto registerUserDto);

        Result Logout(RefreshTokenDto refreshTokenDto);

        Task<ResultWithData<SuccessfulLoginDto>> RefreshTokensAsync(RefreshTokenDto refreshTokenDto);
    }
}