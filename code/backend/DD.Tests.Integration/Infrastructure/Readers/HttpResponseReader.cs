using System.Net.Http.Json;

namespace DD.Tests.Integration.Infrastructure.Readers;

internal static class HttpResponseReader
{
    internal static async Task<T> ReadJsonAsync<T>(
        HttpResponseMessage response,
        string contractName,
        CancellationToken cancellationToken = default)
    {
        return await ReadJsonAsync<T>(
            response,
            contractName,
            ensureSuccess: true,
            cancellationToken);
    }

    internal static async Task<T> ReadJsonAsync<T>(
        HttpResponseMessage response,
        string contractName,
        bool ensureSuccess,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);
        if (ensureSuccess)
            response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken)
               ?? throw new InvalidOperationException(
                   $"The {contractName} response was empty.");
    }

    internal static async Task<string> ReadStringAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadAsStringAsync(cancellationToken)).Trim();
    }
}
