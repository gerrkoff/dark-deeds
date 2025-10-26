using Microsoft.Extensions.Logging;

namespace DD.McpClient.Domain;

internal static partial class Log
{
    [LoggerMessage(
        EventId = 5001,
        Level = LogLevel.Information,
        Message = "Loading tasks by date: {from} to {till}")]
    public static partial void LoadTasks(ILogger logger, DateTime from, DateTime till);

    [LoggerMessage(
        EventId = 5002,
        Level = LogLevel.Information,
        Message = "Updating tasks order for {count} tasks")]
    public static partial void UpdateTasksOrder(ILogger logger, int count);
}
