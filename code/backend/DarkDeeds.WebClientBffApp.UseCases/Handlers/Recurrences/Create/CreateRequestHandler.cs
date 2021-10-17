using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp;
using MediatR;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Recurrences.Create
{
    public class CreateRequestHandler : IRequestHandler<CreateRequestModel, int>
    {
        private readonly ITaskServiceApp _taskServiceApp;

        public CreateRequestHandler(ITaskServiceApp taskServiceApp)
        {
            _taskServiceApp = taskServiceApp;
        }

        public Task<int> Handle(CreateRequestModel request, CancellationToken cancellationToken)
        {
            return _taskServiceApp.CreateRecurrencesAsync(request.TimezoneOffset);
        }
    }
}