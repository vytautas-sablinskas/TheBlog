using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TheBlog.MVC.Annotations;

namespace TheBlog.MVC.ViewModels.Profile
{
    [ExcludeFromCodeCoverage]
    public class UserNewProfileViewModel
    {
        [UniqueUsername(ErrorMessage = "Username is already taken")]
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(30, ErrorMessage = "Username cannot be longer than 30 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
        public string UserName { get; set; }
    }
}