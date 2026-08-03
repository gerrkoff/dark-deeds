using System.Globalization;
using DD.Shared.TaskText;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Time;

namespace DD.TerminalClient.Domain.Editing;

// Renders a task back into the web-compatible editor string ("[date ][time ]title[ flags]") so the
// exact same text round-trips through the shared TaskTextParser. Mirrors the React
// TaskConvertService.convertModelToString: the leading year is elided when it matches the local
// year, the time is only emitted for a dated task, and a single task always produces a single-date
// string (never a range), matching the web single-task edit behavior.
public sealed class TaskTextFormatter(ILocalDateProvider dateProvider)
{
    public string Format(TerminalTask task)
    {
        return FormatCore(task.Date, task.Time, task.Title, task.Type, task.IsProbable);
    }

    public string Format(ParsedTaskText parsed)
    {
        return FormatCore(parsed.Date, parsed.Time, parsed.Title, parsed.Type.ToTerminalTaskType(), parsed.IsProbable);
    }

    private string FormatCore(DateOnly? date, int? time, string title, TerminalTaskType type, bool isProbable)
    {
        var prefix = string.Empty;

        if (date is { } value)
        {
            prefix = FormatDate(value);
            prefix += time is { } minutes ? $" {FormatTime(minutes)} " : " ";
        }

        var suffix = " ";
        suffix += type switch
        {
            TerminalTaskType.Simple => string.Empty,
            TerminalTaskType.Additional => "!",
            TerminalTaskType.Routine => "*",
            TerminalTaskType.Weekly => "%",
            _ => string.Empty,
        };
        if (isProbable)
            suffix += "?";
        if (suffix.Length == 1)
            suffix = string.Empty;

        return $"{prefix}{title}{suffix}";
    }

    private string FormatDate(DateOnly date)
    {
        var result = date.Year != dateProvider.Today.Year
            ? date.Year.ToString(CultureInfo.InvariantCulture)
            : string.Empty;

        return result + TwoDigits(date.Month) + TwoDigits(date.Day);
    }

    private static string FormatTime(int minutes)
    {
        return TwoDigits(minutes / 60) + TwoDigits(minutes % 60);
    }

    private static string TwoDigits(int value)
    {
        return value.ToString("D2", CultureInfo.InvariantCulture);
    }
}
