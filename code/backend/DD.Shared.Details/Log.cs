using Microsoft.Extensions.Logging;

namespace DD.Shared.Details;

internal static partial class Log
{
    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Warning,
        Message = "Failed to notify about task updated, exception: {exceptionMessage}")]
    public static partial void FailedToNotifyAboutTaskUpdated(ILogger logger, string exceptionMessage);
}
