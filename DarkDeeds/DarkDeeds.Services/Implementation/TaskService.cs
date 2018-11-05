using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Models;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.Services.Implementation
{
    public class TaskService : ITaskService
    {
        public async Task<IEnumerable<TaskDto>> LoadTasksAsync()
        {
            return await Task.FromResult(new TaskDto[0]);
        }

        public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks)
        {
            return await Task.FromResult(tasks);
        }
    }
}