using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.Authentication;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Tasks.Save
{
    public class SaveRequestHandler : IRequestHandler<SaveRequestModel, IEnumerable<TaskDto>>
    {
        private readonly ITaskServiceApp _taskServiceApp;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SaveRequestHandler(IHttpContextAccessor httpContextAccessor, ITaskServiceApp taskServiceApp)
        {
            _httpContextAccessor = httpContextAccessor;
            _taskServiceApp = taskServiceApp;
        }

        public Task<IEnumerable<TaskDto>> Handle(SaveRequestModel request, CancellationToken cancellationToken)
        {
            return _taskServiceApp.SaveTasksAsync(request.Tasks,
                _httpContextAccessor.HttpContext.User.ToAuthToken().UserId);
        }
    }
}