using AutoMapper;
using DD.ServiceTask.Domain.Services;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;
using TaskDtoTaskService = DD.ServiceTask.Domain.Dto.TaskDto;

namespace DD.Shared.Details.Common;

public class TaskServiceApp(
    ITaskService taskService,
    ITaskParserService taskParserService,
    IMapper mapper)
    : ITaskServiceApp
{
    public async Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(DateTime from, DateTime till, string userId)
    {
        var response = await taskService.LoadTasksByDateAsync(userId, from, till);
        var result = mapper.Map<IEnumerable<TaskDto>>(response);
        return result;
    }

    public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId)
    {
        var payload = mapper.Map<ICollection<TaskDtoTaskService>>(tasks);
        var response = await taskService.SaveTasksAsync(payload, userId);
        var result = mapper.Map<IEnumerable<TaskDto>>(response);
        return result;
    }

    public Task<TaskDto> ParseTask(string text)
    {
        var response = taskParserService.ParseTask(text);
        var result = mapper.Map<TaskDto>(response);
        return Task.FromResult(result);
    }

    public Task<ICollection<string>> PrintTasks(IEnumerable<TaskDto> tasks)
    {
        var payload = mapper.Map<IEnumerable<TaskDtoTaskService>>(tasks);
        var response = taskParserService.PrintTasks(payload);
        var result = mapper.Map<ICollection<string>>(response);
        return Task.FromResult(result);
    }
}
