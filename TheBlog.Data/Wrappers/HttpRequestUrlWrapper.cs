using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using TheBlog.MVC.Extensions;

namespace TheBlog.MVC.Wrappers
{
    [ExcludeFromCodeCoverage]
    public class HttpRequestUrlWrapper : IHttpRequestUrlWrapper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpRequestUrlWrapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetBaseUrl()
        {
            return _httpContextAccessor.HttpContext.Request.BaseUrl();
        }
    }
}