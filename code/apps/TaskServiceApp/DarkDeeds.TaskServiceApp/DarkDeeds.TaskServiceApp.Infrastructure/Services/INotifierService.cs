using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Models.Dto;

namespace DarkDeeds.TaskServiceApp.Infrastructure.Services
{
    public interface INotifierService
    {
        Task TaskUpdated(ICollection<TaskDto> updatedTasks);
    }
}