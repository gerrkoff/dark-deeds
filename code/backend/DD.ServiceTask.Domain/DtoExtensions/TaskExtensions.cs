using DD.ServiceTask.Domain.Dto;

namespace DD.ServiceTask.Domain.DtoExtensions;

internal static class TaskExtensions
{
    public static ICollection<TaskDto> ToUtcDate(this ICollection<TaskDto> tasks)
    {
        foreach (var task in tasks)
        {
            if (task.Date != null)
                task.Date = DateTime.SpecifyKind(task.Date.Value, DateTimeKind.Utc);
        }

        return tasks;
    }
}
