using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DarkDeeds.Common.Extensions;
using DarkDeeds.Models;
using DarkDeeds.Models.Entity;
using DarkDeeds.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DarkDeeds.Api.Hubs
{
    [Authorize]
    public class TaskHub : Hub
    {
        private readonly ITaskService _taskService;

        public TaskHub(ITaskService taskService)
        {
            _taskService = taskService;
        }
        
        public async Task Save(ICollection<TaskDto> tasks)
        {
            CurrentUser user = ((ClaimsIdentity) Context.User.Identity).ToCurrentUser();
            IEnumerable<TaskDto> updatedTasks = await _taskService.SaveTasksAsync(tasks, user.UserId);
            await Clients.Caller.SendAsync("update", updatedTasks, true);
            await Clients.Others.SendAsync("update", updatedTasks, false);
        }

        public static async Task Update(IHubContext<TaskHub> context, IEnumerable<TaskDto> updatedTasks)
        {
            await context.Clients.All.SendAsync("update", updatedTasks, false);
        }
    }
}