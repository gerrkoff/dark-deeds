namespace DD.TerminalClient.Details.Realtime;

// The exact hub wire contract shared by the real connection and its tests: the same lower-case path,
// client-id query parameter and event names the web client uses. The URL is built from the profile base
// URL (already normalized to one trailing slash) plus the process client id, matched by the server to
// exclude this client's own saves from its update notifications.
internal static class TaskHubProtocol
{
    public const string HubPath = "ws/task/task";

    public const string ClientIdQueryParameter = "clientId";

    public const string UpdateEventName = "update";

    public const string HeartbeatEventName = "heartbeat";

    public static Uri BuildHubUrl(string baseUrl, string clientId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientId);

        var normalized = baseUrl.EndsWith('/') ? baseUrl : baseUrl + "/";
        var query = $"{ClientIdQueryParameter}={Uri.EscapeDataString(clientId)}";
        return new Uri($"{normalized}{HubPath}?{query}");
    }
}
