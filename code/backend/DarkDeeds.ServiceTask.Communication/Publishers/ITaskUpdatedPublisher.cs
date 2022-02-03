using DarkDeeds.ServiceTask.Infrastructure.Services.Dto;

namespace DarkDeeds.ServiceTask.Communication.Publishers
{
    public interface ITaskUpdatedPublisher
    {
        void Send(TaskUpdatedDto updatedTasks);
    }
}