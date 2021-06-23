using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DarkDeeds.Communication.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace DarkDeeds.Communication.Services.Implementation
{
    class DdHttpClientFactory : IDdHttpClientFactory
    {
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DdHttpClientFactory(IHttpContextAccessor httpContextAccessor, IServiceDiscovery serviceDiscovery)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceDiscovery = serviceDiscovery;
        }

        public async Task<HttpClient> Create(string appName)
        {
            var uri = await _serviceDiscovery.GetService(appName);
            
            var httpClient = new HttpClient();
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpClient.BaseAddress = uri;
            return httpClient;
        }
    }
}