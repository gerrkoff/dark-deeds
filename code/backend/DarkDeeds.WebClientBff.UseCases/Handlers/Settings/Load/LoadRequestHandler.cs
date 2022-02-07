using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.Authentication.Core;
using DarkDeeds.WebClientBff.Services.Dto;
using DarkDeeds.WebClientBff.Services.Services.Interface;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Settings.Load
{
    public class LoadRequestHandler : IRequestHandler<LoadRequestModel, SettingsDto>
    {
        private readonly ISettingsService _settingsService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoadRequestHandler(ISettingsService settingsService, IHttpContextAccessor httpContextAccessor)
        {
            _settingsService = settingsService;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<SettingsDto> Handle(LoadRequestModel request, CancellationToken cancellationToken)
        {
            return _settingsService.LoadAsync(_httpContextAccessor.HttpContext.User.ToAuthToken().UserId);
        }
    }
}