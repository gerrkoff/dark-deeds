using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DarkDeeds.Authentication.Core.Services;
using DarkDeeds.Communication.Services.Interface;

namespace DarkDeeds.Communication.Services.Implementation
{
    class DdHttpClientFactory : IDdHttpClientFactory
    {
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly IAuthTokenProvider _authTokenProvider;

        public DdHttpClientFactory(IServiceDiscovery serviceDiscovery, IAuthTokenProvider authTokenProvider)
        {
            _serviceDiscovery = serviceDiscovery;
            _authTokenProvider = authTokenProvider;
        }

        public async Task<HttpClient> Create(string appName)
        {
            var uri = await _serviceDiscovery.GetService(appName);

            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var httpClient = new HttpClient(httpClientHandler)
            {
                BaseAddress = uri,
            };

            var token = await _authTokenProvider.GetToken();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return httpClient;
        }
    }
}