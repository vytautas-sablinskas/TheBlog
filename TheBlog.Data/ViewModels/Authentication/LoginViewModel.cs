using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TheBlog.MVC.Annotations;

namespace TheBlog.MVC.ViewModels.Authentication
{
    [ExcludeFromCodeCoverage]
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        public LoginViewModel(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}