using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.App.Hubs;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using DarkDeeds.WebClientBffApp.Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;

namespace DarkDeeds.WebClientBffApp.App.Services
{
    public class NotifierService : INotifierService
    {
        private readonly IHubContext<TaskHub> _hubContext;

        public NotifierService(IHubContext<TaskHub> hubContext)
        {
            _hubContext = hubContext;
        }
        
        public async Task TaskUpdated(IEnumerable<TaskDto> updatedTasks)
        {
            // TODO: send personal updates, not all
            await _hubContext.Clients.All.SendAsync("update", updatedTasks, false);
        }        
    }
}