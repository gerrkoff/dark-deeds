using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Infrastructure.Communication.Dto;
using DarkDeeds.Models.Dto;

namespace DarkDeeds.Infrastructure.Communication
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
