using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Infrastructure.Services.Dto;

namespace DarkDeeds.TaskServiceApp.Infrastructure.Services
{
    public interface INotifierService
    {
        Task TaskUpdated(TaskUpdatedDto updatedTasks);
    }
}