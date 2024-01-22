using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Consumers;
using DD.TaskService.Domain.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Tasks.Save;

class SaveRequestHandler : IRequestHandler<SaveRequestModel, IEnumerable<TaskDto>>
{
    private readonly ITaskServiceApp _taskServiceApp;

    public SaveRequestHandler(ITaskServiceApp taskServiceApp)
    {
        _taskServiceApp = taskServiceApp;
    }

    public Task<IEnumerable<TaskDto>> Handle(SaveRequestModel request, CancellationToken cancellationToken)
    {
        return _taskServiceApp.SaveTasksAsync(request.Tasks);
    }
}
