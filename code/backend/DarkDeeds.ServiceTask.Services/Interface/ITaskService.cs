using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Dto;

namespace DarkDeeds.ServiceTask.Services.Interface
{
    public interface ITaskService
    {
        /// <summary>
        /// Get all tasks, that:
        /// <para>1. fits current period or in future</para>
        /// <para>2. has no date</para>
        /// <para>3. expired, but not completed</para>
        /// </summary>
        /// <param name="userId">Task's user id</param>
        /// <param name="from">Current period start, UTC expected</param>
        /// <returns>Tasks</returns>
        Task<IEnumerable<TaskDto>> LoadActualTasksAsync(string userId, DateTime from);

        /// <summary>
        /// Get all tasks, filtered by date
        /// </summary>
        /// <param name="userId">Task's user id</param>
        /// <param name="from">Period start, included, UTC expected</param>
        /// <param name="to">Period end, not included, UTC expected</param>
        /// <returns>Tasks</returns>
        Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(string userId, DateTime from, DateTime to);

        Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId);
    }
}
