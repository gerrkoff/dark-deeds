using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using MediatR;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Recurrences.Load
{
    public class LoadRequestHandler : IRequestHandler<LoadRequestModel, IEnumerable<PlannedRecurrenceDto>>
    {
        private readonly ITaskServiceApp _taskServiceApp;

        public LoadRequestHandler(ITaskServiceApp taskServiceApp)
        {
            _taskServiceApp = taskServiceApp;
        }

        public Task<IEnumerable<PlannedRecurrenceDto>> Handle(LoadRequestModel request, CancellationToken cancellationToken)
        {
            return _taskServiceApp.LoadRecurrencesAsync();
        }
    }
}