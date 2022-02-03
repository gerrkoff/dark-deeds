using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.TelegramClient.Infrastructure.Communication.TaskServiceApp.Dto;

namespace DarkDeeds.TelegramClient.Infrastructure.Communication.TaskServiceApp
{
    public interface ITaskServiceApp
    {
        Task<IEnumerable<TaskDto>> LoadTasksByDateAsync(string userId, DateTime from, DateTime to);
        Task<IEnumerable<TaskDto>> SaveTasksAsync(ICollection<TaskDto> tasks, string userId);

        Task<TaskDto> ParseTask(string text);
        Task<ICollection<string>> PrintTasks(IEnumerable<TaskDto> tasks);
    }
}
