using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Consumers;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Recurrences.Save;

class SaveRequestHandler : IRequestHandler<SaveRequestModel, int>
{
    private readonly ITaskServiceApp _taskServiceApp;

    public SaveRequestHandler(ITaskServiceApp taskServiceApp)
    {
        _taskServiceApp = taskServiceApp;
    }

    public Task<int> Handle(SaveRequestModel request, CancellationToken cancellationToken)
    {
        return _taskServiceApp.SaveRecurrencesAsync(request.Recurrences);
    }
}
