using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Communication.Amqp.Subscribe;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using DarkDeeds.WebClientBffApp.Services.Services.Interface;

namespace DarkDeeds.WebClientBffApp.App.BackgroundServices
{
    public class TaskUpdatedSubscriber : AbstractMessageSubscriber<ICollection<TaskDto>>
    {
        private readonly ITaskUpdatedListener _listener;
        
        public TaskUpdatedSubscriber(
            ISubscriber<ICollection<TaskDto>> subscriber, 
            ITaskUpdatedListener listener) 
            : base("notify-task-updated", subscriber)
        {
            _listener = listener;
        }

        protected override Task ProcessMessage(ICollection<TaskDto> message) => _listener.Process(message);
    }
}