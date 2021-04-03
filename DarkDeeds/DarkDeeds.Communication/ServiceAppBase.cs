using System;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace DarkDeeds.Communication
{
    public abstract class ServiceAppBase
    {
        private readonly Lazy<HttpClient> _httpClient;

        protected ServiceAppBase(IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new Lazy<HttpClient>(() =>
            {
                var httpClient = new HttpClient();
                var auth = httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization];
                httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, auth.FirstOrDefault());
                return httpClient;
            });
        }

        protected HttpClient HttpClient => _httpClient.Value;

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