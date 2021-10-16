using DarkDeeds.WebClientBffApp.Services.Dto;
using MediatR;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Settings.Save
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