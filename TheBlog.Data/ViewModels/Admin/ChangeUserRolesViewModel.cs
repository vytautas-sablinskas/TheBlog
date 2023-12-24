using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.MVC.ViewModels.Admin
{
    [ExcludeFromCodeCoverage]
    public class ChangeUserRolesViewModel
    {
        [Required]
        public string UserName { get; set; }

        public List<string> RolesToAdd { get; set; }

        public List<string> RolesToRemove { get; set; }
    }
}