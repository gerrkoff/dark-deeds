using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using DarkDeeds.WebClientBffApp.Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;

namespace DarkDeeds.WebClientBffApp.App.Hubs
{
    public class TaskHubService : ITaskHubService
    {
        private readonly IHubContext<TaskHub> _hubContext;

        public TaskHubService(IHubContext<TaskHub> hubContext)
        {
            _hubContext = hubContext;
        }
        
        public async Task Update(IEnumerable<TaskDto> updatedTasks)
        {
            await _hubContext.Clients.All.SendAsync("update", updatedTasks, false);
        }        
    }
}