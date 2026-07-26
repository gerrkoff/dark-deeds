using Microsoft.Extensions.Logging;

namespace DD.TerminalClient.Details;

// Diagnostics for the real-time hub, emitted only through the injected logger (the interactive build
// wires this to the bounded per-profile file logger, never the console). Messages carry connectivity
// state and a coarse failure reason only - never a token, header or credential.
internal static partial class Log
{
    [LoggerMessage(
        EventId = 7001,
        Level = LogLevel.Debug,
        Message = "Task hub connect attempt failed: {reason}")]
    public static partial void HubConnectAttemptFailed(ILogger logger, string reason);

    [LoggerMessage(
        EventId = 7002,
        Level = LogLevel.Information,
        Message = "Task hub connection lost ({reason}); scheduling reconnect")]
    public static partial void HubConnectionLost(ILogger logger, string reason);

    [LoggerMessage(
        EventId = 7003,
        Level = LogLevel.Information,
        Message = "Task hub reconnected")]
    public static partial void HubReconnected(ILogger logger);

    [LoggerMessage(
        EventId = 7004,
        Level = LogLevel.Warning,
        Message = "Task hub authentication was rejected")]
    public static partial void HubUnauthorized(ILogger logger);

    [LoggerMessage(
        EventId = 7005,
        Level = LogLevel.Information,
        Message = "Task hub connection closed")]
    public static partial void HubClosed(ILogger logger);
}
