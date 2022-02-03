using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.WebClientBff.Infrastructure.Communication.TaskServiceApp.Dto;

namespace DarkDeeds.WebClientBff.Infrastructure.Services
{
    public interface INotifierService
    {
        // TODO: separate model
        Task TaskUpdated(IEnumerable<TaskDto> updatedTasks, string userId);
    }
}