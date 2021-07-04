using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using DarkDeeds.WebClientBffApp.Infrastructure.Services;
using DarkDeeds.WebClientBffApp.Services.Services.Interface;

namespace DarkDeeds.WebClientBffApp.Services.Services.Implementation
{
    class TaskUpdatedListener : ITaskUpdatedListener
    {
        private readonly INotifierService _notifierService;

        public TaskUpdatedListener(INotifierService notifierService)
        {
            _notifierService = notifierService;
        }

        public Task Process(ICollection<TaskDto> model)
        {
            // TODO: remove
            Console.WriteLine("Model: " + model.First().Title);
            return _notifierService.TaskUpdated(model);
        }
    }
}