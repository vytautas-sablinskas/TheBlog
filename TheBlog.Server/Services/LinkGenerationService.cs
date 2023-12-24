using System.Web;
using TheBlog.MVC.Wrappers;

namespace TheBlog.MVC.Services
{
    public class LinkGenerationService : ILinkGenerationService
    {
        private readonly IHttpRequestUrlWrapper _httpRequestUrlWrapper;

        public LinkGenerationService(IHttpRequestUrlWrapper httpRequestUrlWrapper)
        {
            _httpRequestUrlWrapper = httpRequestUrlWrapper;
        }

        public async Task<string> GeneratePasswordResetLink(string token, string email)
        {
            string baseUrl = _httpRequestUrlWrapper.GetBaseUrl();
            var encodedEmail = HttpUtility.UrlEncode(email);
            var encodedToken = HttpUtility.UrlEncode(token);

            var resetLink = $"{baseUrl}reset-password?token={encodedToken}&email={encodedEmail}";
            return resetLink;
        }
    }
}