using DarkDeeds.Communication.Amqp.Publish;
using DD.TaskService.Domain.Dto;

namespace DarkDeeds.ServiceTask.Communication.Publishers;

class TaskUpdatedPublisher : AbstractMessagePublisher<TaskUpdatedDto>, ITaskUpdatedPublisher
{
    public TaskUpdatedPublisher(IPublisher<TaskUpdatedDto> publisher) : base("notify-task-updated", publisher)
    {
    }

    public void Send(TaskUpdatedDto updatedTasks)
    {
        Publish(new PublishItem<TaskUpdatedDto>(updatedTasks));
    }
}
