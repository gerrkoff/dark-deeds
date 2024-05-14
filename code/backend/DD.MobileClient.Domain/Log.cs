using Microsoft.Extensions.Logging;

namespace DD.MobileClient.Domain;

internal static partial class Log
{
    [LoggerMessage(
        EventId = 4001,
        Level = LogLevel.Information,
        Message = "Missed app status cache for MobileKey={MobileKey}")]
    public static partial void MissedAppStatusCache(ILogger logger, string mobileKey);

    [LoggerMessage(
        EventId = 4002,
        Level = LogLevel.Information,
        Message = "Missed app status cache for MobileKey={MobileKey}")]
    public static partial void MissedWidgetStatusCache(ILogger logger, string mobileKey);
}
