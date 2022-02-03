using DarkDeeds.WebClientBff.Services.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Settings.Load
{
    public class LoadRequestModel : IRequest<SettingsDto>
    {
    }
}