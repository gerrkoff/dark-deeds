using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Dto;

namespace DarkDeeds.ServiceTask.Consumers
{
    public interface ITaskServiceApp
    {
        Task<IEnumerable<TaskDto>> LoadActualTasksAsync(DateTime from, string userId = null);
        Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(DateTime from, DateTime to, string userId = null);
        Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId = null);

        Task<int> CreateRecurrencesAsync(int timezoneOffset);
        Task<IEnumerable<PlannedRecurrenceDto>> LoadRecurrencesAsync();
        Task<int> SaveRecurrencesAsync(ICollection<PlannedRecurrenceDto> recurrences);

        Task<TaskDto> ParseTask(string text);
        Task<ICollection<string>> PrintTasks(IEnumerable<TaskDto> tasks);
    }
}
