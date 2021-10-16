using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp;
using MediatR;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Recurrences.Save
{
    public class SaveRequestHandler : IRequestHandler<SaveRequestModel, int>
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
}