using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;

namespace DarkDeeds.WebClientBffApp.Infrastructure.Services
{
    public interface INotifierService
    {
        // TODO: separate model
        Task TaskUpdated(IEnumerable<TaskDto> updatedTasks, string userId);
    }
}