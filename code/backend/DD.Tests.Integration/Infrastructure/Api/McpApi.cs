namespace DD.Tests.Integration.Infrastructure.Api;

internal static class McpApi
{
    internal static readonly Uri Endpoint = new("http://localhost/mcp");

    private static readonly Uri RelativeEndpoint = new("/mcp", UriKind.Relative);

    internal static Task<HttpResponseMessage> GetAsync(
        HttpClient client,
        CancellationToken cancellationToken = default)
    {
        return client.GetAsync(RelativeEndpoint, cancellationToken);
    }
}
