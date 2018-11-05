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
            var list = new List<TaskDto>
            {
                new TaskDto{Id = 1},
                new TaskDto{Id = 2}
            };
            return await Task.FromResult(list);
        }

        public async Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks)
        {
            return await Task.FromResult(tasks);
        }
    }
}