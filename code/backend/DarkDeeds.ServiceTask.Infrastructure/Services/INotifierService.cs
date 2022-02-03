using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Infrastructure.Services.Dto;

namespace DarkDeeds.ServiceTask.Infrastructure.Services
{
    public interface INotifierService
    {
        Task TaskUpdated(TaskUpdatedDto updatedTasks);
    }
}