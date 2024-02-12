using DD.ServiceTask.Domain.Dto;
using DD.ServiceTask.Domain.Services;
using DD.Shared.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DD.ServiceTask.Details.Web.Hubs;

[Authorize]
public class TaskHub(
    IAuthTokenConverter authTokenConverter,
    ITaskService taskService)
    : Hub
{
    // TODO: remove
    public async Task Save(ICollection<TaskDto> tasks)
    {
        ArgumentNullException.ThrowIfNull(Context.User);

        var userId = authTokenConverter.FromPrincipal(Context.User).UserId;
        var updatedTasks = await taskService.SaveTasksAsync(tasks, userId);
        await Clients.Caller.SendAsync("update", updatedTasks, true);
        await Clients.Others.SendAsync("update", updatedTasks, false);
    }
}
