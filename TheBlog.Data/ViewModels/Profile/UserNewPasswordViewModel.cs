using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.MVC.ViewModels.Profile
{
    [ExcludeFromCodeCoverage]
    public class UserNewPasswordViewModel
    {
        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [RegularExpression(@"^(?=.*\W)(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?!.*\s).+$", ErrorMessage = "Password must contain at least one non-alphanumeric character, one digit, one uppercase letter, and one lowercase letter without spaces.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
        public string ConfirmPassword { get; set; }
    }
}