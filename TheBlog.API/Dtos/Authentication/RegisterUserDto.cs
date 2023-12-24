using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TheBlog.MVC.Annotations;

namespace TheBlog.API.Dtos.Authentication
{
    [ExcludeFromCodeCoverage]
    public record RegisterUserDto(
        [UniqueUsername(ErrorMessage = "Username is already taken")]
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(30, ErrorMessage = "Username cannot be longer than 30 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
        string Username,

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        string Email,

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [RegularExpression(@"^(?=.*\W)(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?!.*\s).+$", ErrorMessage = "Password must contain at least one non-alphanumeric character, one digit, one uppercase letter, and one lowercase letter without spaces.")]
        [DataType(DataType.Password)]
        string Password
    );
}