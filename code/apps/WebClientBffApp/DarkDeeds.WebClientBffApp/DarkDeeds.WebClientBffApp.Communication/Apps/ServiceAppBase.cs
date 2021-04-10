using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace DarkDeeds.WebClientBffApp.Communication
{
    public abstract class ServiceAppBase
    {
        private readonly Lazy<Task<HttpClient>> _httpClient;

        protected ServiceAppBase(IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new Lazy<Task<HttpClient>>(async () =>
            {
                var httpClient = new HttpClient();
                var token = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return httpClient;
            });
        }

        protected Task<HttpClient> HttpClient => _httpClient.Value;

        protected HttpContent SerializePayload<T>(T payload) =>
            new StringContent(JsonSerializer.Serialize(payload, _jsonOptions), Encoding.UTF8,
                MediaTypeNames.Application.Json);

        protected async Task<T> ParseBodyAsync<T>(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(body, _jsonOptions);
        }

        protected string DateToString(DateTime dateTime) =>
            dateTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
        
        
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }
}