using Microsoft.AspNetCore.Identity;

namespace TheBlog.MVC.Wrappers
{
    public interface IRoleManagerWrapper
    {
        Task<IdentityResult> CreateAsync(IdentityRole role);
        Task<bool> RoleExistsAsync(string role);
    }
}