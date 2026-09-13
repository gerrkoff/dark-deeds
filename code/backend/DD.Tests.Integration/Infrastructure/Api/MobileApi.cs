namespace DD.Tests.Integration.Infrastructure.Api;

internal static class MobileApi
{
    private const string WatchRoute = "api/mobile/Watch";

    internal static Task<HttpResponseMessage> GetWidgetAsync(
        HttpClient client,
        string mobileKey,
        CancellationToken cancellationToken = default)
    {
        return client.GetAsync(CreateWatchUri(mobileKey, "widget"), cancellationToken);
    }

    internal static Task<HttpResponseMessage> GetAppAsync(
        HttpClient client,
        string mobileKey,
        CancellationToken cancellationToken = default)
    {
        return client.GetAsync(CreateWatchUri(mobileKey, "app"), cancellationToken);
    }

    internal static Task<HttpResponseMessage> CreateUserMappingAsync(
        HttpClient client,
        string userId,
        string mobileKey,
        CancellationToken cancellationToken = default)
    {
        var uri = new Uri(
            $"api/test/CreateMobileUserMapping?userId={Uri.EscapeDataString(userId)}&mobileKey={Uri.EscapeDataString(mobileKey)}",
            UriKind.Relative);
        return client.PostAsync(uri, content: null, cancellationToken);
    }

    private static Uri CreateWatchUri(string mobileKey, string endpoint)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mobileKey);
        return new Uri(
            $"{WatchRoute}/{Uri.EscapeDataString(mobileKey)}/{endpoint}",
            UriKind.Relative);
    }
}
