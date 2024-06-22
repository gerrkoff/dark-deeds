using AutoMapper;
using DD.ServiceTask.Domain.Infrastructure;
using DD.Shared.Details.Abstractions.Dto;
using TaskUpdatedDtoTaskService = DD.ServiceTask.Domain.Dto.TaskUpdatedDto;

namespace DD.Shared.Details.Common;

public class TaskServiceNotifier(
    IMapper mapper,
    ITaskServiceNotifierChannelProvider channelProvider) : INotifierService
{
    public async Task TaskUpdated(TaskUpdatedDtoTaskService updatedTasks)
    {
        var updatedTasksDto = mapper.Map<TasksUpdatedDto>(updatedTasks);
        await channelProvider.WriteAsync(updatedTasksDto);
    }
}
