using System.Threading.Tasks;
using DarkDeeds.Authentication.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace DarkDeeds.Authentication.Services
{
    class AuthTokenProvider : IAuthTokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthTokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<string> GetToken()
        {   
            return _httpContextAccessor.HttpContext?.GetTokenAsync("access_token");
        }
    }
}