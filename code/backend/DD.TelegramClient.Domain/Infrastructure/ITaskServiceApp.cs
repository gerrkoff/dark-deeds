using DD.TelegramClient.Domain.Infrastructure.Dto;

namespace DD.TelegramClient.Domain.Infrastructure;

public interface ITaskServiceApp
{
    Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(DateTime from, DateTime to, string userId);
    Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId);

    Task<TaskDto> ParseTask(string text);
    Task<ICollection<string>> PrintTasks(IEnumerable<TaskDto> tasks);
}
