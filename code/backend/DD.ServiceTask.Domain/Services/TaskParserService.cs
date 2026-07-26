using DD.ServiceTask.Domain.Exceptions;
using DD.Shared.Details.Abstractions.Dto;
using DD.Shared.TaskText;

namespace DD.ServiceTask.Domain.Services;

public interface ITaskParserService
{
    IReadOnlyList<TaskDto> ParseTasks(string task);

    TaskDto ParseTaskTemplate(string task);
}

// Thin adapter that maps shared task-text parse results onto the transport TaskDto contract.
// All parsing, range validation and expansion live in DD.Shared.TaskText.TaskTextParser.
public class TaskParserService(ITaskTextParser parser) : ITaskParserService
{
    public IReadOnlyList<TaskDto> ParseTasks(string task)
    {
        try
        {
            return parser.Parse(task).Select(ToDto).ToList();
        }
        catch (TaskTextParseException e)
        {
            throw new ServiceException(e.Message, e);
        }
    }

    public TaskDto ParseTaskTemplate(string task)
    {
        return ToDto(parser.ParseTemplate(task));
    }

    private static TaskDto ToDto(ParsedTaskText parsed)
    {
        return new TaskDto
        {
            Title = parsed.Title,
            Type = MapType(parsed.Type),
            IsProbable = parsed.IsProbable,
            Time = parsed.Time,
            Date = parsed.Date is { } date
                ? DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc)
                : null,
        };
    }

    private static TaskTypeDto MapType(TaskTextType type)
    {
        return type switch
        {
            TaskTextType.Simple => TaskTypeDto.Simple,
            TaskTextType.Additional => TaskTypeDto.Additional,
            TaskTextType.Routine => TaskTypeDto.Routine,
            TaskTextType.Weekly => TaskTypeDto.Weekly,
            _ => TaskTypeDto.Simple,
        };
    }
}
