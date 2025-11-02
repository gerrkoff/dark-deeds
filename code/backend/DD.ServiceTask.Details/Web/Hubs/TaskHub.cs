using DD.ServiceTask.Details.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DD.ServiceTask.Details.Web.Hubs;

[Authorize]
public class TaskHub(IClientConnectionTracker clientConnectionTracker) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var clientId = Context.GetHttpContext()?.Request.Query["clientId"].FirstOrDefault();
        clientConnectionTracker.AddConnection(Context.ConnectionId, clientId);

        var userId = Context.UserIdentifier;
        if (!string.IsNullOrWhiteSpace(userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);

        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        clientConnectionTracker.RemoveConnection(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
