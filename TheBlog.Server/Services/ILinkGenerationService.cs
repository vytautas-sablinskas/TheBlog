namespace TheBlog.MVC.Services
{
    public interface ILinkGenerationService
    {
        Task<string> GeneratePasswordResetLink(string token, string email);
    }
}
