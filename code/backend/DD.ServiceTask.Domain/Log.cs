using Microsoft.Extensions.Logging;

namespace DD.ServiceTask.Domain;

public static partial class Log
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Warning,
        Message = "Can't parse EveryMonthDay for PlannedRecurrenceUid = {PlannedRecurrenceUid}, Value = '{PlannedRecurrenceEveryMonthDay}'")]
    public static partial void FailedToParseMonthWorkDay(ILogger logger, string plannedRecurrenceUid, string plannedRecurrenceEveryMonthDay);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Warning,
        Message = "Tried to update foreign task. TaskUid: {TaskUid} User: {UserId}")]
    public static partial void TriedToUpdateForeignTask(ILogger logger, string taskUid, string userId);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Warning,
        Message = "Tried to delete non existing task. TaskUid: {TaskUid}")]
    public static partial void TriedToDeleteNonExistingTask(ILogger logger, string taskUid);
}
