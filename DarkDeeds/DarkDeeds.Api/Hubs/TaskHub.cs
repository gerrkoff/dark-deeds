using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DarkDeeds.Auth.Extensions;
using DarkDeeds.Auth.Models;
using DarkDeeds.Models.Dto;
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
    }
}