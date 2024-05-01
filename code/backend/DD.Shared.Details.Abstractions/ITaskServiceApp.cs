using DD.Shared.Details.Abstractions.Dto;

namespace DD.Shared.Details.Abstractions;

public interface ITaskServiceApp
{
    Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(DateTime from, DateTime till, string userId);

    Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId);

    Task<TaskDto> ParseTask(string text);

    Task<ICollection<string>> PrintTasks(IEnumerable<TaskDto> tasks);
}
