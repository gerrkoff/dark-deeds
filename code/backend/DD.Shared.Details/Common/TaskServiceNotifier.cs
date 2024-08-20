using DD.ServiceTask.Domain.Infrastructure;
using DD.Shared.Details.Abstractions.Dto;

namespace DD.Shared.Details.Common;

public class TaskServiceNotifier(ITaskServiceNotifierChannelProvider channelProvider) : INotifierService
{
    public async Task TaskUpdated(TasksUpdatedDto updatedTasks)
    {
        await channelProvider.WriteAsync(updatedTasks);
    }
}