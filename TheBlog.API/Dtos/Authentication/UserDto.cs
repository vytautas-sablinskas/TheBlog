using System.Diagnostics.CodeAnalysis;

namespace TheBlog.API.Dtos.Authentication
{
    [ExcludeFromCodeCoverage]
    public record UserDto(string Id, string UserName, string Email);
}