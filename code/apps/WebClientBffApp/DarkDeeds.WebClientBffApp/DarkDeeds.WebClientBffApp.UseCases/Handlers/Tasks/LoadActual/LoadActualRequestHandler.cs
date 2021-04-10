using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.Authentication;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Tasks.LoadActual
{
    public class LoadActualRequestHandler : IRequestHandler<LoadActualRequestModel, IEnumerable<TaskDto>>
    {
        private readonly ITaskServiceApp _taskServiceApp;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoadActualRequestHandler(IHttpContextAccessor httpContextAccessor, ITaskServiceApp taskServiceApp)
        {
            _httpContextAccessor = httpContextAccessor;
            _taskServiceApp = taskServiceApp;
        }

        public Task<IEnumerable<TaskDto>> Handle(LoadActualRequestModel request, CancellationToken cancellationToken)
        {
            return _taskServiceApp.LoadActualTasksAsync(_httpContextAccessor.HttpContext.User.ToAuthToken().UserId,
                request.From);
        }
    }
}