using DD.MobileClient.Domain.Infrastructure.Dto;

namespace DD.MobileClient.Domain.Infrastructure;

public interface ITaskServiceApp
{
    Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(DateTime from, DateTime till, string userId);

    Task<ICollection<string>> PrintTasks(IEnumerable<TaskDto> tasks);
}
