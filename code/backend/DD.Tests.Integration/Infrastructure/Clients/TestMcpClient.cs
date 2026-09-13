using Microsoft.Extensions.Logging.Abstractions;
using ModelContextProtocol.Client;
using DD.Tests.Integration.Infrastructure.Api;
using SdkMcpClient = ModelContextProtocol.Client.McpClient;

namespace DD.Tests.Integration.Infrastructure.Clients;

internal sealed class TestMcpClient : IAsyncDisposable
{
    private TestMcpClient(SdkMcpClient client)
    {
        Client = client;
    }

    public SdkMcpClient Client { get; }

    public static async Task<TestMcpClient> CreateAsync(
        TestOAuthClient oauthClient,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(oauthClient);
        var transport = new HttpClientTransport(
            new HttpClientTransportOptions
            {
                Endpoint = McpApi.Endpoint,
                TransportMode = HttpTransportMode.StreamableHttp,
                ConnectionTimeout = TimeSpan.FromSeconds(5),
            },
            oauthClient.HttpClient,
            loggerFactory: NullLoggerFactory.Instance,
            ownsHttpClient: false);

        try
        {
            var client = await SdkMcpClient.CreateAsync(
                transport,
                cancellationToken: cancellationToken);
            return new TestMcpClient(client);
        }
        catch
        {
            await transport.DisposeAsync();
            throw;
        }
    }

    public ValueTask DisposeAsync()
    {
        return Client.DisposeAsync();
    }
}
