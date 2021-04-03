using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp.Dto;

namespace DarkDeeds.TelegramClientApp.Infrastructure.Services
{
    public interface ITaskHubService
    {
        Task Update(IEnumerable<TaskDto> updatedTasks);
    }
}