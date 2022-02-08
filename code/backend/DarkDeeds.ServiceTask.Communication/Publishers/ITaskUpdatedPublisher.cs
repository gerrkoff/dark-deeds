using DarkDeeds.ServiceTask.Infrastructure.Services.Dto;

namespace DarkDeeds.ServiceTask.Communication.Publishers
{
    interface ITaskUpdatedPublisher
    {
        void Send(TaskUpdatedDto updatedTasks);
    }
}