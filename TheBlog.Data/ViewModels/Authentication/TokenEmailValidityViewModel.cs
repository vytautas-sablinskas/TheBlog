using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.MVC.ViewModels.Authentication
{
    [ExcludeFromCodeCoverage]
    public class TokenEmailValidityViewModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string Email { get; set; }
    }
}