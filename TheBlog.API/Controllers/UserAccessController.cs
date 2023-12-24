using Microsoft.AspNetCore.Mvc;
using TheBlog.API.Dtos.Authentication;
using TheBlog.API.Services;

namespace TheBlog.API.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class UserAccessController : ControllerBase
    {
        private readonly IUserAuthenticationService _userAuthenticationService;

        public UserAccessController(IUserAuthenticationService userAuthenticationService)
        {
            _userAuthenticationService = userAuthenticationService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _userAuthenticationService.LoginAsync(loginDto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterUserDto registerDto)
        {
            var result = await _userAuthenticationService.RegisterAsync(registerDto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return CreatedAtAction(nameof(Register), result.Value);
        }

        [HttpPost]
        [Route("logout")]
        public IActionResult Logout(RefreshTokenDto refreshTokenDto)
        {
            var result = _userAuthenticationService.Logout(refreshTokenDto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpPost]
        [Route("refreshTokens")]
        public async Task<IActionResult> RefreshTokens(RefreshTokenDto refreshTokenDto)
        {
            var result = await _userAuthenticationService.RefreshTokensAsync(refreshTokenDto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Value);
        }
    }
}