using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;

namespace DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp
{
    public interface ITaskServiceApp
    {
        Task<IEnumerable<TaskDto>> LoadActualTasksAsync(DateTime from);
        Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks);

        Task<int> CreateRecurrencesAsync(int timezoneOffset);
        Task<IEnumerable<PlannedRecurrenceDto>> LoadRecurrencesAsync();
        Task<int> SaveRecurrencesAsync(ICollection<PlannedRecurrenceDto> recurrences);
    }
}
