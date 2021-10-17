using DarkDeeds.TaskServiceApp.Infrastructure.Services.Dto;

namespace DarkDeeds.TaskServiceApp.Communication.Publishers
{
    public interface ITaskUpdatedPublisher
    {
        void Send(TaskUpdatedDto updatedTasks);
    }
}