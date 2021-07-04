using System.Collections.Generic;
using DarkDeeds.Communication.Amqp.Publish;
using DarkDeeds.TaskServiceApp.Models.Dto;

namespace DarkDeeds.TaskServiceApp.Communication.Publishers
{
    public class TaskUpdatedPublisher : AbstractMessagePublisher<ICollection<TaskDto>>, ITaskUpdatedPublisher 
    {
        public TaskUpdatedPublisher(IPublisher<ICollection<TaskDto>> publisher) : base("notify-task-updated", publisher)
        {
        }

        public void Send(ICollection<TaskDto> tasks)
        {
            Publish(new PublishItem<ICollection<TaskDto>>(tasks));
        }
    }
}