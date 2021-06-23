using System;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DarkDeeds.Communication
{
    public static class HttpExtensions
    {
        public static async Task<HttpContent> SerializeBodyAsync<T>(T payload, CancellationToken cancellationToken = default)
        {
            var ms = new MemoryStream();
            await JsonSerializer.SerializeAsync(ms, payload, JsonOptions, cancellationToken);
            return new StringContent(ms.ToString(), Encoding.UTF8, MediaTypeNames.Application.Json);
        }

        public static async Task<T> DeserializeBodyAsync<T>(this HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            var body = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(body, JsonOptions, cancellationToken);
        }

        public static string Serialize(DateTime dateTime) =>
            dateTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
        
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }
}