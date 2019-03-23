using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Models;
using DarkDeeds.Models.Entity;

namespace DarkDeeds.Services.Interface
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> LoadTasksAsync(string userId);
        Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId);
    }
}