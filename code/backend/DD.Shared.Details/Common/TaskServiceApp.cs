using DD.ServiceTask.Domain.Services;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;

namespace DD.Shared.Details.Common;

public class TaskServiceApp(
    ITaskService taskService,
    ITaskParserService taskParserService)
    : ITaskServiceApp
{
    public Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(DateTime from, DateTime till, string userId)
    {
        return taskService.LoadTasksByDateAsync(userId, from, till);
    }

    public Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId)
    {
        return taskService.SaveTasksAsync(tasks, userId);
    }

    public Task<TaskDto> ParseTask(string text)
    {
        return Task.FromResult(taskParserService.ParseTask(text));
    }
}
