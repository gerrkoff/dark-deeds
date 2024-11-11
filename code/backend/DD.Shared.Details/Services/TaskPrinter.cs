using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;

namespace DD.Shared.Details.Services;

public class TaskPrinter : ITaskPrinter
{
    public string PrintWithSymbolCodes(TaskDto task)
    {
        var result = string.Empty;

        if (task.Time.HasValue)
            result += $"{TimeToString(task.Time.Value)} ";

        result += task.Title;

        if (task.Type == TaskTypeDto.Additional)
            result += " !";
        else if (task.Type == TaskTypeDto.Routine)
            result += " *";

        if (task.IsProbable)
        {
            if (task.Type == TaskTypeDto.Simple)
                result += " ";
            result += "?";
        }

        return result;
    }

    public string PrintContent(TaskDto task)
    {
        var result = string.Empty;
        if (task.Time.HasValue)
            result += $"{TimeToString(task.Time.Value)} ";

        result += task.Title;

        return result;
    }

    private static string TimeToString(int time)
    {
        var hour = time / 60;
        var minute = time % 60;
        return $"{hour:D2}:{minute:D2}";
    }
}
