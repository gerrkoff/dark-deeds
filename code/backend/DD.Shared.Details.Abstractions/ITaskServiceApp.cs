using DD.Shared.Details.Abstractions.Dto;

namespace DD.Shared.Details.Abstractions;

public interface ITaskServiceApp
{
    Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(DateTime from, DateTime till, string userId);

    Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId, string? clientId);

    Task<IEnumerable<TaskDto>> UpdateTasksAsync(ICollection<TaskUpdateDto> updates, string userId, string? clientId);

    Task<TaskDto> ParseTask(string text);
}
