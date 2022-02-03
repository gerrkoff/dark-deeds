using DarkDeeds.WebClientBff.Services.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Settings.Save
{
    public class SaveRequestModel : IRequest
    {
        public SaveRequestModel(SettingsDto settings)
        {
            Settings = settings;
        }

        public SettingsDto Settings { get; }
    }
}