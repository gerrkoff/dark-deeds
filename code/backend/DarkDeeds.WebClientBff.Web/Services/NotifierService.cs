using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.WebClientBff.Web.Hubs;
using DD.TaskService.Domain.Dto;
using Microsoft.AspNetCore.SignalR;

namespace DarkDeeds.WebClientBff.Web.Services;

public class NotifierService(IHubContext<TaskHub> hubContext)
{
    public async Task TaskUpdated(IEnumerable<TaskDto> updatedTasks, string userId)
    {
        await hubContext.Clients.User(userId)
            .SendAsync("update", updatedTasks, false);
    }
}
