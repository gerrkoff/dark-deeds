namespace DD.Tests.Integration.Infrastructure.Api;

internal static class SignalRApi
{
    private static readonly Uri TaskHubNegotiateUri =
        new("ws/task/task/negotiate?negotiateVersion=1", UriKind.Relative);

    internal static Task<HttpResponseMessage> NegotiateTaskHubAsync(
        HttpClient client,
        CancellationToken cancellationToken = default)
    {
        return client.PostAsync(TaskHubNegotiateUri, content: null, cancellationToken);
    }
}
