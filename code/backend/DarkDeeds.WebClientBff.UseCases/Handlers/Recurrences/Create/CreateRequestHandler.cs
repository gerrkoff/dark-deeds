using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Consumers;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Recurrences.Create
{
    class CreateRequestHandler : IRequestHandler<CreateRequestModel, int>
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
