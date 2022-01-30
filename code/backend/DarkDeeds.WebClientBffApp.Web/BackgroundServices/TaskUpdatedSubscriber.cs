using System.Threading.Tasks;
using DarkDeeds.Communication.Amqp.Subscribe;
using DarkDeeds.WebClientBffApp.Services.Dto;
using DarkDeeds.WebClientBffApp.Services.Services.Interface;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.WebClientBffApp.Web.BackgroundServices
{
    public class TaskUpdatedSubscriber : AbstractMessageSubscriber<TaskUpdatedDto>
    {
        private readonly ITaskUpdatedListener _listener;
        
        public TaskUpdatedSubscriber(
            ISubscriber<TaskUpdatedDto> subscriber, 
            ITaskUpdatedListener listener, 
            ILogger<TaskUpdatedSubscriber> logger) 
            : base("notify-task-updated", subscriber, logger)
        {
            _listener = listener;
        }

        protected override Task ProcessMessage(TaskUpdatedDto message) => _listener.Process(message);
    }
}