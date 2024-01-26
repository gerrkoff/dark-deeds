using System.Threading.Tasks;
using DarkDeeds.Communication.Amqp.Subscribe;
using DarkDeeds.WebClientBff.Web.Models;
using DarkDeeds.WebClientBff.Web.Services;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.WebClientBff.Web.BackgroundServices;

public class TaskUpdatedSubscriber(
    ISubscriber<TaskUpdatedDto> subscriber,
    NotifierService notifierService,
    ILogger<TaskUpdatedSubscriber> logger)
    : AbstractMessageSubscriber<TaskUpdatedDto>("notify-task-updated", subscriber, logger)
{
    protected override Task ProcessMessage(TaskUpdatedDto message) =>
        notifierService.TaskUpdated(message.Tasks, message.UserId);
}
