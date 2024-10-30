using DD.ServiceTask.Details.Web.Hubs;
using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Dto;
using Microsoft.AspNetCore.SignalR;

namespace DD.ServiceTask.Details.Subscriptions;

public class TaskServiceSubscriber(IHubContext<TaskHub> hubContext) : ITaskServiceSubscriber
{
    public Task TasksUpdated(TasksUpdatedDto tasksUpdated)
    {
        return hubContext.Clients.User(tasksUpdated.UserId)
            .SendAsync("update", tasksUpdated.Tasks);
    }
}
