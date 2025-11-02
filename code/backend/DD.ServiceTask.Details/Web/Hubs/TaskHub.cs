using DD.ServiceTask.Details.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DD.ServiceTask.Details.Web.Hubs;

[Authorize]
public class TaskHub(IClientConnectionTracker clientConnectionTracker) : Hub
{
    public override Task OnConnectedAsync()
    {
        var clientId = Context.GetHttpContext()?.Request.Query["clientId"].FirstOrDefault();
        clientConnectionTracker.AddConnection(Context.ConnectionId, clientId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        clientConnectionTracker.RemoveConnection(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
