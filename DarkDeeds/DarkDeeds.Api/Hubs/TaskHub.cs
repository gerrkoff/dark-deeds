using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Authentication;
using DarkDeeds.Authentication.Models;
using DarkDeeds.Infrastructure.Communication;
using DarkDeeds.Infrastructure.Communication.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DarkDeeds.Api.Hubs
{
    [Authorize]
    public class TaskHub : Hub
    {
        private readonly ITaskServiceApp _taskServiceApp;

        public TaskHub(ITaskServiceApp taskServiceApp)
        {
            _taskServiceApp = taskServiceApp;
        }
        
        public async Task Save(ICollection<TaskDto> tasks)
        {
            CurrentUser user = Context.User.ToCurrentUser();
            IEnumerable<TaskDto> updatedTasks = await _taskServiceApp.SaveTasksAsync(tasks, user.UserId);
            await Clients.Caller.SendAsync("update", updatedTasks, true);
            await Clients.Others.SendAsync("update", updatedTasks, false);
        }
    }
}