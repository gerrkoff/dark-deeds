using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DarkDeeds.WebClientBffApp.App.Hubs
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
            IEnumerable<TaskDto> updatedTasks = await _taskServiceApp.SaveTasksAsync(tasks);
            await Clients.Caller.SendAsync("update", updatedTasks, true);
            await Clients.Others.SendAsync("update", updatedTasks, false);
        }
    }
}