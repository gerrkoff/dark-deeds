using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Consumers;
using DD.TaskService.Domain.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Recurrences.Load;

class LoadRequestHandler : IRequestHandler<LoadRequestModel, IEnumerable<PlannedRecurrenceDto>>
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
