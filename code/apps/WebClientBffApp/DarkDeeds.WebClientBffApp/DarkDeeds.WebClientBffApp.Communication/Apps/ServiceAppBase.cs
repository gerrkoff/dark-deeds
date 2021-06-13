using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DarkDeeds.Communication.Services.Interface;

namespace DarkDeeds.WebClientBffApp.Communication.Apps
{
    public abstract class ServiceAppBase
    {
        private readonly IDdHttpClientFactory _clientFactory;
        
        protected ServiceAppBase(IDdHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        protected abstract string AppName { get; }
        
        protected Task<HttpClient> HttpClient => _clientFactory.Create(AppName);

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