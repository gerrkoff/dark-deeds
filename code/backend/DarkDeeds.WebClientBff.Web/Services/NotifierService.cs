using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Dto;
using DarkDeeds.WebClientBff.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DarkDeeds.WebClientBff.Web.Services
{
    public class NotifierService
    {
        private readonly IHubContext<TaskHub> _hubContext;

        public NotifierService(IHubContext<TaskHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task TaskUpdated(IEnumerable<TaskDto> updatedTasks, string userId)
        {
            await _hubContext.Clients.User(userId)
                .SendAsync("update", updatedTasks, false);
        }
    }
}
