using DD.ServiceTask.Details.Web.Hubs;
using DD.ServiceTask.Domain.Dto;
using DD.ServiceTask.Domain.Infrastructure;
using Microsoft.AspNetCore.SignalR;

namespace DD.ServiceTask.Details.Infrastructure;

public class NotifierService(IHubContext<TaskHub> hubContext) : INotifierService
{
    public Task TaskUpdated(TaskUpdatedDto updatedTasks)
    {
        return hubContext.Clients.User(updatedTasks.UserId)
            .SendAsync("update", updatedTasks.Tasks, false);
    }
}
