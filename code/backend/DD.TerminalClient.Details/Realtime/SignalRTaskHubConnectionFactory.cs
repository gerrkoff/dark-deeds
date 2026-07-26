using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace DD.TerminalClient.Details.Realtime;

// Builds the real SignalR connection for a profile: the exact hub URL with the process client id, a
// JSON protocol matching the server's camel-cased payloads, and an access-token provider that reads the
// current JWT afresh on every negotiate. The base URL and client id are fixed per profile; the token is
// resolved lazily so it always reflects the latest renewal.
internal sealed class SignalRTaskHubConnectionFactory : ITaskHubConnectionFactory
{
    private readonly string _baseUrl;
    private readonly string _clientId;

    public SignalRTaskHubConnectionFactory(string baseUrl, string clientId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientId);
        _baseUrl = baseUrl;
        _clientId = clientId;
    }

    public ITaskHubConnection Create(Func<string?> tokenProvider)
    {
        ArgumentNullException.ThrowIfNull(tokenProvider);

        var url = TaskHubProtocol.BuildHubUrl(_baseUrl, _clientId);
        var connection = new HubConnectionBuilder()
            .WithUrl(url, options => options.AccessTokenProvider = () => Task.FromResult<string?>(tokenProvider()))
            .AddJsonProtocol(options =>
                options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase)
            .Build();

        return new SignalRTaskHubConnection(connection);
    }
}
