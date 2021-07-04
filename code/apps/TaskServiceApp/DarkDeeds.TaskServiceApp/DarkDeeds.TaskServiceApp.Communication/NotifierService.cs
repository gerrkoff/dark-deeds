using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Communication.Publishers;
using DarkDeeds.TaskServiceApp.Infrastructure.Services;
using DarkDeeds.TaskServiceApp.Models.Dto;

namespace DarkDeeds.TaskServiceApp.Communication
{
    public class NotifierService : INotifierService
    {
        private readonly ITaskUpdatedPublisher _taskUpdatedPublisher;

        public NotifierService(ITaskUpdatedPublisher taskUpdatedPublisher)
        {
            _taskUpdatedPublisher = taskUpdatedPublisher;
        }

        public Task TaskUpdated(ICollection<TaskDto> updatedTasks)
        {
            // TODO: change to ICollection
            _taskUpdatedPublisher.Send(updatedTasks);
            return Task.CompletedTask;
        }
    }
}