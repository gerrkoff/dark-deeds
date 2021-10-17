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
    // TODO: refactor!
    public static class HttpExtensions
    {
        public static async Task<string> SerializeAsync<T>(T payload, CancellationToken cancellationToken = default)
        {
            await using var ms = new MemoryStream();
            await JsonSerializer.SerializeAsync(ms, payload, JsonOptions, cancellationToken);
            ms.Position = 0;
            using var sr = new StreamReader(ms);
            var result = await sr.ReadToEndAsync();
            return result;
        }
        
        public static async Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, cancellationToken);
        }
        
        public static async Task<HttpContent> SerializeBodyAsync<T>(T payload, CancellationToken cancellationToken = default)
        {
            var serialized = await SerializeAsync(payload, cancellationToken);
            return new StringContent(serialized, Encoding.UTF8, MediaTypeNames.Application.Json);
        }

        public static async Task<T> DeserializeBodyAsync<T>(this HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            var body = await response.Content.ReadAsStreamAsync();
            return await DeserializeAsync<T>(body, cancellationToken);
        }

        public static string Serialize(DateTime dateTime) =>
            dateTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
        
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }
}