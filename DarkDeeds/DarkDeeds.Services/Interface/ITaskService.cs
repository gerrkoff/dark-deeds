using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Models;

namespace DarkDeeds.Services.Interface
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> LoadTasksAsync(string userId, DateTime? from = null, DateTime? to = null);
        Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId);
    }
}