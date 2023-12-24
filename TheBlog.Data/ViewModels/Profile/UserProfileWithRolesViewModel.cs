using System.Diagnostics.CodeAnalysis;

namespace TheBlog.MVC.ViewModels.Profile
{
    [ExcludeFromCodeCoverage]
    public class UserProfileWithRolesViewModel
    {
        public string UserName { get; set; }

        public List<string> Roles { get; set; }
    }
}