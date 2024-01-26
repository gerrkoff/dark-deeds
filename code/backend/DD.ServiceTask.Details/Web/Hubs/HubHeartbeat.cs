using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace DD.ServiceTask.Details.Web.Hubs;

public class HubHeartbeat<T>(IHubContext<T> hubContext) : BackgroundService
    where T : Hub
{
    private const int HeartbeatTimer = 60 * 1000;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await hubContext.Clients.All.SendAsync("heartbeat", stoppingToken);
            await Task.Delay(HeartbeatTimer, stoppingToken);
        }
    }
}
