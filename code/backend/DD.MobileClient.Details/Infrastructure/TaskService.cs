using AutoMapper;
using DD.MobileClient.Domain.Infrastructure;
using DD.ServiceTask.Domain.Services;
using TaskDtoMobileClient = DD.MobileClient.Domain.Infrastructure.Dto.TaskDto;
using TaskDtoTaskService = DD.ServiceTask.Domain.Dto.TaskDto;

namespace DD.MobileClient.Details.Infrastructure;

public class TaskServiceApp(
    ITaskService taskService,
    ITaskParserService taskParserService,
    IMapper mapper)
    : ITaskServiceApp
{
    public async Task<IEnumerable<TaskDtoMobileClient>> LoadTasksByDateAsync(DateTime from, DateTime till, string userId)
    {
        var response = await taskService.LoadTasksByDateAsync(userId, from, till);
        var result = mapper.Map<IEnumerable<TaskDtoMobileClient>>(response);
        return result;
    }

    public Task<ICollection<string>> PrintTasks(IEnumerable<TaskDtoMobileClient> tasks)
    {
        var payload = mapper.Map<IEnumerable<TaskDtoTaskService>>(tasks);
        var response = taskParserService.PrintTasks(payload);
        var result = mapper.Map<ICollection<string>>(response);
        return Task.FromResult(result);
    }
}
