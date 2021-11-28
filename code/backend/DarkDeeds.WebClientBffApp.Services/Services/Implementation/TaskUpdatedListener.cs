using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Services;
using DarkDeeds.WebClientBffApp.Services.Dto;
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

        public Task Process(TaskUpdatedDto model)
        {
            return _notifierService.TaskUpdated(model.Tasks, model.UserId);
        }
    }
}