using Microsoft.Extensions.Logging;

namespace DD.ServiceTask.Domain;

internal static partial class Log
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

    [LoggerMessage(
        EventId = 1004,
        Level = LogLevel.Warning,
        Message = "Tried to update non existing task. TaskUid: {TaskUid}")]
    public static partial void TriedToUpdateNonExistingTask(ILogger logger, string taskUid);

    [LoggerMessage(
        EventId = 1005,
        Level = LogLevel.Warning,
        Message = "Tried to update deleted task. TaskUid: {TaskUid}")]
    public static partial void TriedToUpdateDeletedTask(ILogger logger, string taskUid);

    [LoggerMessage(
        EventId = 1006,
        Level = LogLevel.Warning,
        Message = "Update task version conflict. TaskUid: {TaskUid}")]
    public static partial void UpdateTaskVersionConflict(ILogger logger, string taskUid);

    [LoggerMessage(
        EventId = 1007,
        Level = LogLevel.Warning,
        Message = "Failed to update recurrence TaskUid. PlannedRecurrenceUid: {PlannedRecurrenceUid}, Date: {Date}, TaskUid: {TaskUid}")]
    public static partial void FailedToUpdateRecurrenceTaskUid(ILogger logger, string plannedRecurrenceUid, DateTime date, string taskUid);
}
