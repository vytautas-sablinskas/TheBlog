using System.Diagnostics.CodeAnalysis;

namespace TheBlog.API.Dtos.Authentication
{
    [ExcludeFromCodeCoverage]
    public record SuccessfulLoginDto(
        string AccessToken,

        string RefreshToken
    );
}