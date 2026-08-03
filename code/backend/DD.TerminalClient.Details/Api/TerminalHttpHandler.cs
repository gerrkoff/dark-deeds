using System.Net.Http.Headers;

namespace DD.TerminalClient.Details.Api;

// The one place outbound REST requests are stamped with identity. It attaches the same process client
// id used by the SignalR hub on every request, and a bearer Authorization header whenever a token is
// currently available. The token is read fresh per request through the accessor, so a renewal or a
// logout takes effect on the very next call without rebuilding the handler chain. Anonymous calls (like
// sign-in) simply carry no Authorization header when the accessor returns null.
public sealed class TerminalHttpHandler(Func<string?> tokenAccessor, string clientId) : DelegatingHandler
{
    internal const string ClientIdHeader = "X-Client-Id";

    private readonly Func<string?> _tokenAccessor =
        tokenAccessor ?? throw new ArgumentNullException(nameof(tokenAccessor));

    private readonly string _clientId = string.IsNullOrWhiteSpace(clientId)
        ? throw new ArgumentException("Client id must not be empty.", nameof(clientId))
        : clientId;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Headers.Remove(ClientIdHeader);
        request.Headers.Add(ClientIdHeader, _clientId);

        var token = _tokenAccessor();
        request.Headers.Authorization = string.IsNullOrWhiteSpace(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);

        return base.SendAsync(request, cancellationToken);
    }
}
