using AutoMapper;
using DD.ServiceTask.Domain.Infrastructure;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;
using Microsoft.Extensions.Logging;
using TaskUpdatedDtoTaskService = DD.ServiceTask.Domain.Dto.TaskUpdatedDto;

namespace DD.Shared.Details.Common;

public class TaskServiceNotifier(
    IEnumerable<ITaskServiceSubscriber> taskServiceNotificationSubscribers,
    IMapper mapper,
    ILogger<TaskServiceNotifier> logger) : INotifierService
{
    private readonly List<ITaskServiceSubscriber> _subscribers = taskServiceNotificationSubscribers.ToList();

    public async Task TaskUpdated(TaskUpdatedDtoTaskService updatedTasks)
    {
        var updatedTasksDto = mapper.Map<TasksUpdatedDto>(updatedTasks);
        foreach (var subscriber in _subscribers)
        {
            try
            {
                await subscriber.TasksUpdated(updatedTasksDto);
            }
#pragma warning disable CA1031
            catch (Exception ex)
#pragma warning restore CA1031
            {
                Log.FailedToNotifyAboutTaskUpdated(logger, ex.Message);
            }
        }
    }
}
