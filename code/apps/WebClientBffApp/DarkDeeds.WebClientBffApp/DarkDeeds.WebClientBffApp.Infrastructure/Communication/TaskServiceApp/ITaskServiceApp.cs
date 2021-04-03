using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;

namespace DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp
{
    public interface ITaskServiceApp
    {
        Task<IEnumerable<TaskDto>> LoadActualTasksAsync(string userId, DateTime from);
        Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(string userId, DateTime from, DateTime to);
        Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId);

        Task<int> CreateRecurrencesAsync(int timezoneOffset, string userId);
        Task<IEnumerable<PlannedRecurrenceDto>> LoadRecurrencesAsync(string userId);
        Task<int> SaveRecurrencesAsync(ICollection<PlannedRecurrenceDto> recurrences, string userId);

        Task<TaskDto> ParseTask(string text);
        Task<string> PrintTasks(IEnumerable<TaskDto> tasks);
    }
}
