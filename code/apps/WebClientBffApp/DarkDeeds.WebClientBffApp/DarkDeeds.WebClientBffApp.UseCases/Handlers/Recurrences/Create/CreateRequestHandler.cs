using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.Authentication;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Recurrences.Create
{
    public class CreateRequestHandler : IRequestHandler<CreateRequestModel, int>
    {
        private readonly ITaskServiceApp _taskServiceApp;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateRequestHandler(ITaskServiceApp taskServiceApp, IHttpContextAccessor httpContextAccessor)
        {
            _taskServiceApp = taskServiceApp;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<int> Handle(CreateRequestModel request, CancellationToken cancellationToken)
        {
            return _taskServiceApp.CreateRecurrencesAsync(request.TimezoneOffset,
                _httpContextAccessor.HttpContext.User.ToAuthToken().UserId);
        }
    }
}