using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace DarkDeeds.WebClientBffApp.App.Hubs
{
    public class HubHeartbeat<T> : BackgroundService where T: Hub
    {
        private const int HeartbeatTimer = 60 * 1000;
        
        private readonly IHubContext<T> _hubContext;

        public HubHeartbeat(IHubContext<T> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _hubContext.Clients.All.SendAsync("heartbeat", stoppingToken);
                await Task.Delay(HeartbeatTimer, stoppingToken);
            }
        }
    }
}