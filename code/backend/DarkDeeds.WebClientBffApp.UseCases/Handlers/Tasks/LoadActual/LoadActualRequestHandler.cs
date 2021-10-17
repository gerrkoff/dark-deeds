using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using MediatR;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Tasks.LoadActual
{
    public class LoadActualRequestHandler : IRequestHandler<LoadActualRequestModel, IEnumerable<TaskDto>>
    {
        private readonly ITaskServiceApp _taskServiceApp;

        public LoadActualRequestHandler(ITaskServiceApp taskServiceApp)
        {
            _taskServiceApp = taskServiceApp;
        }

        public Task<IEnumerable<TaskDto>> Handle(LoadActualRequestModel request, CancellationToken cancellationToken)
        {
            return _taskServiceApp.LoadActualTasksAsync(request.From);
        }
    }
}