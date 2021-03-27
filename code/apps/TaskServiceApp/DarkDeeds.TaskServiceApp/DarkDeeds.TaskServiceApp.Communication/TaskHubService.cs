using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Infrastructure.Services;
using DarkDeeds.TaskServiceApp.Models.Dto;

namespace DarkDeeds.TaskServiceApp.Communication
{
    public class TaskHubService : ITaskHubService
    {
        public Task Update(IEnumerable<TaskDto> updatedTasks) => Task.CompletedTask;
    }
}