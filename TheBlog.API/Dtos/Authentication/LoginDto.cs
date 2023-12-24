using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.API.Dtos.Authentication
{
    [ExcludeFromCodeCoverage]
    public record LoginDto(
        [Required]
        string Username,

        [Required]
        string Password
    );
}