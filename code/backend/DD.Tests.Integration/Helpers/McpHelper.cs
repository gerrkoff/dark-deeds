using System.Net.Http.Headers;
using DD.Tests.Integration.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using ModelContextProtocol.Client;
using SdkMcpClient = ModelContextProtocol.Client.McpClient;

namespace DD.Tests.Integration.Helpers;

internal sealed class McpTestClient : IAsyncDisposable
{
    private readonly HttpClient _httpClient;

    private McpTestClient(HttpClient httpClient, SdkMcpClient client)
    {
        _httpClient = httpClient;
        Client = client;
    }

    public SdkMcpClient Client { get; }

    public static async Task<McpTestClient> ConnectAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var httpClient = await IntegrationEnvironmentLifetime.CreateNoRedirectClientAsync();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var transport = new HttpClientTransport(
            new HttpClientTransportOptions
            {
                Endpoint = new Uri("http://localhost/mcp"),
                TransportMode = HttpTransportMode.StreamableHttp,
                ConnectionTimeout = TimeSpan.FromSeconds(5),
            },
            httpClient,
            loggerFactory: NullLoggerFactory.Instance,
            ownsHttpClient: false);

        try
        {
            var client = await SdkMcpClient.CreateAsync(transport, cancellationToken: cancellationToken);
            return new McpTestClient(httpClient, client);
        }
        catch
        {
            await transport.DisposeAsync();
            httpClient.Dispose();
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await Client.DisposeAsync();
        _httpClient.Dispose();
    }
}
