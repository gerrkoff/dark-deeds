using DD.MobileClient.Domain.Dto;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;

namespace DD.MobileClient.Domain.Services;

public interface IWatchPayloadController
{
    WatchWidgetStatusDto GetWidgetStatus(IReadOnlyCollection<TaskDto> tasks);

    WatchAppStatusDto GetAppStatus(IReadOnlyCollection<TaskDto> tasks);
}

public class WatchPayloadController(ITaskPrinter taskPrinter) : IWatchPayloadController
{
    public WatchWidgetStatusDto GetWidgetStatus(IReadOnlyCollection<TaskDto> tasks)
    {
        tasks = FilterTasksForStatus(tasks).ToList();

        var header = GetHeader(tasks);

        var firstNotCompletedSimple = tasks.FirstOrDefault(task => task is { Type: TaskTypeDto.Simple });
        var firstNotCompleted = tasks.FirstOrDefault();

        var main = firstNotCompletedSimple != null
            ? taskPrinter.PrintContent(firstNotCompletedSimple)
            : string.Empty;

        var support = firstNotCompleted != null && firstNotCompletedSimple != firstNotCompleted
            ? taskPrinter.PrintContent(firstNotCompleted)
            : string.Empty;

        return new WatchWidgetStatusDto(
            header,
            main,
            support);
    }

    public WatchAppStatusDto GetAppStatus(IReadOnlyCollection<TaskDto> tasks)
    {
        tasks = FilterTasksForStatus(tasks).ToList();

        var header = GetHeader(tasks);

        var items = tasks
            .Select(task => new WatchAppStatusItemDto(
                taskPrinter.PrintContent(task),
                task.Type == TaskTypeDto.Routine))
            .ToList();

        return new WatchAppStatusDto(
            header,
            items);
    }

    public static IEnumerable<TaskDto> FilterTasksForStatus(IEnumerable<TaskDto> tasks)
    {
        return tasks
            .Where(x => !x.Completed && x.Type != TaskTypeDto.Additional)
            .OrderBy(x => x.Order);
    }

    private static string GetHeader(IReadOnlyCollection<TaskDto> tasks)
    {
        var remaining = tasks.Count(task => task is { Completed: false, Type: TaskTypeDto.Simple });
        var header = remaining == 0
            ? "🎉 all finished!"
            : $"\ud83d\udccc {remaining} remaining";

        return header;
    }
}
