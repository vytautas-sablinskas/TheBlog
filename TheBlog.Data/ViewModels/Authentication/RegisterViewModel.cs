using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TheBlog.MVC.Annotations;

namespace TheBlog.MVC.ViewModels.Authentication
{
    [ExcludeFromCodeCoverage]
    public class RegisterViewModel
    {
        [UniqueUsername(ErrorMessage = "Username is already taken")]
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(30, ErrorMessage = "Username cannot be longer than 30 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [RegularExpression(@"^(?=.*\W)(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?!.*\s).+$", ErrorMessage = "Password must contain at least one non-alphanumeric character, one digit, one uppercase letter, and one lowercase letter without spaces.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
        public string ConfirmPassword { get; set; }
    }
}