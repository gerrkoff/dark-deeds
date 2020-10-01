using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Api.Hubs;
using DarkDeeds.Infrastructure.Services;
using DarkDeeds.Models.Dto;
using Microsoft.AspNetCore.SignalR;

namespace DarkDeeds.Api.InfrastructureServices
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