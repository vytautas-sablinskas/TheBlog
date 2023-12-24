using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.API.Dtos.Authentication
{
    [ExcludeFromCodeCoverage]
    public record RefreshTokenDto(
        [Required(AllowEmptyStrings = false)]
        string RefreshToken
    );
}