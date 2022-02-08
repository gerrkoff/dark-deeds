using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.Authentication.Core;
using DarkDeeds.WebClientBff.Services.Services.Interface;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Settings.Save
{
    class SaveRequestHandler : AsyncRequestHandler<SaveRequestModel>
    {
        private readonly ISettingsService _settingsService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SaveRequestHandler(ISettingsService settingsService, IHttpContextAccessor httpContextAccessor)
        {
            _settingsService = settingsService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task Handle(SaveRequestModel request, CancellationToken cancellationToken)
        {
            return _settingsService.SaveAsync(request.Settings,
                _httpContextAccessor.HttpContext.User.ToAuthToken().UserId);
        }
    }
}