using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Communication.Publishers;
using DarkDeeds.TaskServiceApp.Infrastructure.Services;
using DarkDeeds.TaskServiceApp.Infrastructure.Services.Dto;

namespace DarkDeeds.TaskServiceApp.Communication
{
    public class NotifierService : INotifierService
    {
        private readonly ITaskUpdatedPublisher _taskUpdatedPublisher;

        public NotifierService(ITaskUpdatedPublisher taskUpdatedPublisher)
        {
            _taskUpdatedPublisher = taskUpdatedPublisher;
        }

        public Task TaskUpdated(TaskUpdatedDto updatedTasks)
        {
            // TODO: change to ICollection
            _taskUpdatedPublisher.Send(updatedTasks);
            return Task.CompletedTask;
        }
    }
}