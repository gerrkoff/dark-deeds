using AutoMapper;
using DD.ServiceTask.Domain.Services;
using DD.TelegramClient.Domain.Infrastructure;
using TaskDtoTelegramClient = DD.TelegramClient.Domain.Infrastructure.Dto.TaskDto;
using TaskDtoTaskService = DD.ServiceTask.Domain.Dto.TaskDto;

namespace DD.TelegramClient.Details.Infrastructure;

public class TaskServiceApp(
    ITaskService taskService,
    ITaskParserService taskParserService,
    IMapper mapper)
    : ITaskServiceApp
{
    public async Task<IEnumerable<TaskDtoTelegramClient>> LoadTasksByDateAsync(DateTime from, DateTime till, string userId)
    {
        var response = await taskService.LoadTasksByDateAsync(userId, from, till);
        var result = mapper.Map<IEnumerable<TaskDtoTelegramClient>>(response);
        return result;
    }

    public async Task<IEnumerable<TaskDtoTelegramClient>> SaveTasksAsync(ICollection<TaskDtoTelegramClient> tasks, string userId)
    {
        var payload = mapper.Map<ICollection<TaskDtoTaskService>>(tasks);
        var response = await taskService.SaveTasksAsync(payload, userId);
        var result = mapper.Map<IEnumerable<TaskDtoTelegramClient>>(response);
        return result;
    }

    public Task<TaskDtoTelegramClient> ParseTask(string text)
    {
        var response = taskParserService.ParseTask(text);
        var result = mapper.Map<TaskDtoTelegramClient>(response);
        return Task.FromResult(result);
    }

    public Task<ICollection<string>> PrintTasks(IEnumerable<TaskDtoTelegramClient> tasks)
    {
        var payload = mapper.Map<IEnumerable<TaskDtoTaskService>>(tasks);
        var response = taskParserService.PrintTasks(payload);
        var result = mapper.Map<ICollection<string>>(response);
        return Task.FromResult(result);
    }
}
