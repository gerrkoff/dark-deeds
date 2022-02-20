using System.Threading.Tasks;
using DarkDeeds.Communication.Amqp.Subscribe;
using DarkDeeds.WebClientBff.Web.Models;
using DarkDeeds.WebClientBff.Web.Services;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.WebClientBff.Web.BackgroundServices
{
    public class TaskUpdatedSubscriber : AbstractMessageSubscriber<TaskUpdatedDto>
    {
        private readonly NotifierService _notifierService;

        public TaskUpdatedSubscriber(
            ISubscriber<TaskUpdatedDto> subscriber,
            NotifierService notifierService,
            ILogger<TaskUpdatedSubscriber> logger)
            : base("notify-task-updated", subscriber, logger)
        {
            _notifierService = notifierService;
        }

        protected override Task ProcessMessage(TaskUpdatedDto message) =>
            _notifierService.TaskUpdated(message.Tasks, message.UserId);
    }
}
