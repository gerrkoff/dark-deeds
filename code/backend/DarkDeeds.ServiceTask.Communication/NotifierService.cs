using System;
using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Communication.Publishers;
using DarkDeeds.ServiceTask.Infrastructure.Services;
using DarkDeeds.ServiceTask.Infrastructure.Services.Dto;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.ServiceTask.Communication
{
    class NotifierService : INotifierService
    {
        private readonly ITaskUpdatedPublisher _taskUpdatedPublisher;
        private readonly ILogger<NotifierService> _logger;

        public NotifierService(ITaskUpdatedPublisher taskUpdatedPublisher, ILogger<NotifierService> logger)
        {
            _taskUpdatedPublisher = taskUpdatedPublisher;
            _logger = logger;
        }

        public Task TaskUpdated(TaskUpdatedDto updatedTasks)
        {
            try
            {
                // TODO: change to ICollection
                _taskUpdatedPublisher.Send(updatedTasks);
            }
            catch (Exception e)
            {
                // TODO: specify exception?
                _logger.LogWarning($"Failed to notify about updated tasks. Exception message: {e.Message}", e);
            }

            return Task.CompletedTask;
        }
    }
}