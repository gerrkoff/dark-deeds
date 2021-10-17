using DarkDeeds.Communication.Amqp.Publish;
using DarkDeeds.TaskServiceApp.Infrastructure.Services.Dto;

namespace DarkDeeds.TaskServiceApp.Communication.Publishers
{
    public class TaskUpdatedPublisher : AbstractMessagePublisher<TaskUpdatedDto>, ITaskUpdatedPublisher 
    {
        public TaskUpdatedPublisher(IPublisher<TaskUpdatedDto> publisher) : base("notify-task-updated", publisher)
        {
        }

        public void Send(TaskUpdatedDto updatedTasks)
        {
            Publish(new PublishItem<TaskUpdatedDto>(updatedTasks));
        }
    }
}