using DD.Shared.Details.Abstractions.Dto;
using DD.TerminalClient.Domain.Models;

namespace DD.TerminalClient.Details.Api;

// Maps between the terminal task model and the shared TaskDto transport contract. Dates cross the wire
// by UTC calendar components only: a local DateOnly becomes that same calendar day at UTC midnight, and
// an incoming instant is read back by its UTC year/month/day. This mirrors the web client exactly, so a
// change of SSH or client timezone can never shift a task to an adjacent day.
internal static class TaskTransportMapper
{
    public static TaskDto ToDto(TerminalTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        return new TaskDto
        {
            Uid = task.Uid,
            Title = task.Title,
            Date = ToTransportDate(task.Date),
            Time = task.Time,
            Order = task.Order,
            Completed = task.Completed,
            Deleted = task.Deleted,
            IsProbable = task.IsProbable,
            Type = ToDtoType(task.Type),
            Version = task.Version,
        };
    }

    public static TerminalTask ToTerminalTask(TaskDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new TerminalTask
        {
            Uid = dto.Uid,
            Title = dto.Title,
            Date = FromTransportDate(dto.Date),
            Time = dto.Time,
            Order = dto.Order,
            Completed = dto.Completed,
            Deleted = dto.Deleted,
            IsProbable = dto.IsProbable,
            Type = ToTerminalType(dto.Type),
            Version = dto.Version,
        };
    }

    public static DateTime ToTransportInstant(DateOnly date)
    {
        return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
    }

    private static DateTime? ToTransportDate(DateOnly? date)
    {
        return date is { } value ? ToTransportInstant(value) : null;
    }

    private static DateOnly? FromTransportDate(DateTime? date)
    {
        if (date is not { } value)
        {
            return null;
        }

        // Treat an unspecified-kind instant as UTC and convert any local instant to UTC first, so the
        // resulting calendar day is always the server's UTC day regardless of the process timezone.
        var utc = value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
            : value.ToUniversalTime();

        return DateOnly.FromDateTime(utc);
    }

    private static TaskTypeDto ToDtoType(TerminalTaskType type)
    {
        return type switch
        {
            TerminalTaskType.Simple => TaskTypeDto.Simple,
            TerminalTaskType.Additional => TaskTypeDto.Additional,
            TerminalTaskType.Routine => TaskTypeDto.Routine,
            TerminalTaskType.Weekly => TaskTypeDto.Weekly,
            _ => TaskTypeDto.Simple,
        };
    }

    private static TerminalTaskType ToTerminalType(TaskTypeDto type)
    {
        return type switch
        {
            TaskTypeDto.Simple => TerminalTaskType.Simple,
            TaskTypeDto.Additional => TerminalTaskType.Additional,
            TaskTypeDto.Routine => TerminalTaskType.Routine,
            TaskTypeDto.Weekly => TerminalTaskType.Weekly,
            _ => TerminalTaskType.Simple,
        };
    }
}
