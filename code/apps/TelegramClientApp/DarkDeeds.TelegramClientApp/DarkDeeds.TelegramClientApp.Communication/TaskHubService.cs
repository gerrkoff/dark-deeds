using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp.Dto;
using DarkDeeds.TelegramClientApp.Infrastructure.Services;

namespace DarkDeeds.TelegramClientApp.Communication
{
    public class TaskHubService : ITaskHubService
    {
        public Task Update(IEnumerable<TaskDto> updatedTasks) => Task.CompletedTask;
    }
}