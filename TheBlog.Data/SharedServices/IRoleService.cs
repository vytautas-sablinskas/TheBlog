namespace TheBlog.MVC.Services
{
    public interface IRoleService
    {
        Task<bool> AddRolesToUserAsync(string userId, List<string> rolesToAdd);

        Task<bool> RemoveRolesFromUserAsync(string userId, List<string> rolesToRemove);

        Task<List<string>> GetUserRolesByUsernameAsync(string username);

        Task<List<string>> GetUserRolesByUserIdAsync(string userId);
    }
}