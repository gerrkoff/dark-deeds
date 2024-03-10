using Microsoft.Extensions.Logging;

namespace DD.TelegramClient.Domain;

internal static partial class Log
{
    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Warning,
        Message = "Command processing failed. Command: {Command}")]
    public static partial void FailedToProcessCommand(ILogger logger, object command);
}
