using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.Authentication;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Recurrences.Load
{
    public class LoadRequestHandler : IRequestHandler<LoadRequestModel, IEnumerable<PlannedRecurrenceDto>>
    {
        private readonly ITaskServiceApp _taskServiceApp;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoadRequestHandler(ITaskServiceApp taskServiceApp, IHttpContextAccessor httpContextAccessor)
        {
            _taskServiceApp = taskServiceApp;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<IEnumerable<PlannedRecurrenceDto>> Handle(LoadRequestModel request, CancellationToken cancellationToken)
        {
            return _taskServiceApp.LoadRecurrencesAsync(_httpContextAccessor.HttpContext.User.ToAuthToken().UserId);
        }
    }
}