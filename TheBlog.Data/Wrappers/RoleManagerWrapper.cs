using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace TheBlog.MVC.Wrappers
{
    [ExcludeFromCodeCoverage]
    public class RoleManagerWrapper : IRoleManagerWrapper
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleManagerWrapper(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<bool> RoleExistsAsync(string role)
        {
            return await _roleManager.RoleExistsAsync(role);
        }

        public async Task<IdentityResult> CreateAsync(IdentityRole role)
        {
            return await _roleManager.CreateAsync(role);
        }
    }
}