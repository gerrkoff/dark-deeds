namespace DD.Tests.Integration.Infrastructure.Api;

internal static class SystemApi
{
    private static readonly Uri BuildInfoUri = new("api/be/build-info", UriKind.Relative);
    private static readonly Uri HealthUri = new("healthcheck", UriKind.Relative);

    internal static Task<HttpResponseMessage> GetHealthAsync(
        HttpClient client,
        CancellationToken cancellationToken = default)
    {
        return client.GetAsync(HealthUri, cancellationToken);
    }

    internal static Task<HttpResponseMessage> GetBuildInfoAsync(
        HttpClient client,
        CancellationToken cancellationToken = default)
    {
        return client.GetAsync(BuildInfoUri, cancellationToken);
    }
}
