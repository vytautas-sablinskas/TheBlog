using TheBlog.Data.Entities;
using TheBlog.Data.Utilities;
using TheBlog.MVC.Wrappers;

namespace TheBlog.MVC.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUserManagerWrapper<User> _userManagerWrapper;

        public RoleService(IUserManagerWrapper<User> userManagerWrapper)
        {
            _userManagerWrapper = userManagerWrapper;
        }

        public async Task<bool> AddRolesToUserAsync(string username, List<string> rolesToAdd)
        {
            var user = await _userManagerWrapper.FindByNameAsync(username);
            if (user == null)
            {
                return false;
            }

            var hasAddedAllRoles = true;
            foreach (var role in rolesToAdd)
            {
                var userHasRole = await UserHasRoleAsync(user, role);
                if (RoleExists(role) && !userHasRole)
                {
                    var result = await _userManagerWrapper.AddToRoleAsync(user, role);

                    if (!result.Succeeded)
                    {
                        hasAddedAllRoles = false;
                    }
                }
            }

            return hasAddedAllRoles;
        }

        public async Task<bool> RemoveRolesFromUserAsync(string username, List<string> rolesToRemove)
        {
            var user = await _userManagerWrapper.FindByNameAsync(username);
            if (user == null)
            {
                return false;
            }

            var hasRemovedAllRoles = true;
            foreach (var role in rolesToRemove)
            {
                var userHasRole = await UserHasRoleAsync(user, role);
                if (userHasRole)
                {
                    var result = await _userManagerWrapper.RemoveFromRoleAsync(user, role);

                    if (!result.Succeeded)
                    {
                        hasRemovedAllRoles = false;
                    }
                }
            }

            return hasRemovedAllRoles;
        }

        public async Task<List<string>> GetUserRolesByUsernameAsync(string username)
        {
            var user = await _userManagerWrapper.FindByNameAsync(username);
            if (user == null)
            {
                return new List<string>();
            }

            return (await _userManagerWrapper.GetRolesAsync(user)).ToList();
        }

        public async Task<List<string>> GetUserRolesByUserIdAsync(string userId)
        {
            var user = await _userManagerWrapper.FindByIdAsync(userId);
            if (user == null)
            {
                return new List<string>();
            }

            return (await _userManagerWrapper.GetRolesAsync(user)).ToList();
        }

        private bool RoleExists(string roleName)
        {
            return AppRoles.Roles.Contains(roleName);
        }

        private async Task<bool> UserHasRoleAsync(User user, string roleName)
        {
            return await _userManagerWrapper.IsInRoleAsync(user, roleName);
        }
    }
}