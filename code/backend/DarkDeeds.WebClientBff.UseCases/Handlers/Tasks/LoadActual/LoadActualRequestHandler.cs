using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Consumers;
using DarkDeeds.ServiceTask.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Tasks.LoadActual
{
    class LoadActualRequestHandler : IRequestHandler<LoadActualRequestModel, IEnumerable<TaskDto>>
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
