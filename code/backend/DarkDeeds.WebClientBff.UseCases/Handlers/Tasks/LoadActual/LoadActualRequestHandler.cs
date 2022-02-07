using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.WebClientBff.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.WebClientBff.Infrastructure.Communication.TaskServiceApp.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Tasks.LoadActual
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