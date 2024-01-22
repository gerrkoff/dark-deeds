using DD.TaskService.Domain.Dto;

namespace DarkDeeds.ServiceTask.Communication.Publishers;

interface ITaskUpdatedPublisher
{
    void Send(TaskUpdatedDto updatedTasks);
}
