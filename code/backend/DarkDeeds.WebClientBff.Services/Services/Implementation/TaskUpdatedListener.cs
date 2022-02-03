using System.Threading.Tasks;
using DarkDeeds.WebClientBff.Infrastructure.Services;
using DarkDeeds.WebClientBff.Services.Dto;
using DarkDeeds.WebClientBff.Services.Services.Interface;

namespace DarkDeeds.WebClientBff.Services.Services.Implementation
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