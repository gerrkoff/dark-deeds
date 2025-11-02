using DD.ServiceTask.Details.Web.Hubs;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;
using Microsoft.AspNetCore.SignalR;

namespace DD.ServiceTask.Details.Subscriptions;

public class TaskServiceSubscriber(
    IHubContext<TaskHub> hubContext,
    IHubClientConnectionTracker hubClientConnectionTracker) : ITaskServiceSubscriber
{
    public Task TasksUpdated(TasksUpdatedDto tasksUpdated)
    {
        var excludeConnectionIds = !string.IsNullOrWhiteSpace(tasksUpdated.ClientId)
            ? hubClientConnectionTracker.GetConnectionIdsByClientId(tasksUpdated.ClientId)
            : [];

        return hubContext.Clients
            .GroupExcept(tasksUpdated.UserId, excludeConnectionIds)
            .SendAsync("update", tasksUpdated.Tasks);
    }
}
