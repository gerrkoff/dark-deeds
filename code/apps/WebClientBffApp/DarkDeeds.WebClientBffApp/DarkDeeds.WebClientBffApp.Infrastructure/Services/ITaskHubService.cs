using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;

namespace DarkDeeds.WebClientBffApp.Infrastructure.Services
{
    public interface ITaskHubService
    {
        Task Update(IEnumerable<TaskDto> updatedTasks);
    }
}