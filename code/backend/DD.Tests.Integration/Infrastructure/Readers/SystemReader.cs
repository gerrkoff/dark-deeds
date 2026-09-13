using System.Text.Json;

namespace DD.Tests.Integration.Infrastructure.Readers;

internal static class SystemReader
{
    internal static Task<string> ReadHealthAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadStringAsync(response, cancellationToken);
    }

    internal static Task<JsonDocument> ReadBuildInfoAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadJsonAsync<JsonDocument>(
            response,
            cancellationToken);
    }
}
