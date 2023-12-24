using Microsoft.AspNetCore.Identity;
using TheBlog.Data.Utilities;
using TheBlog.MVC.Wrappers;

namespace TheBlog.MVC.Services
{
    public class RoleSeederService : IRoleSeederService
    {
        private readonly IRoleManagerWrapper _roleManagerWrapper;

        public RoleSeederService(IRoleManagerWrapper roleManagerWrapper)
        {
            _roleManagerWrapper = roleManagerWrapper;
        }

        public async Task SeedRolesAsync()
        {
            foreach (var role in AppRoles.Roles)
            {
                var roleExists = await _roleManagerWrapper.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await _roleManagerWrapper.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}