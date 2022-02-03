using DarkDeeds.WebClientBff.Infrastructure.Communication.TelegramClientApp.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Telegram.Start
{
    public class StartRequestModel : IRequest<TelegramStartDto>
    {
        public StartRequestModel(int timezoneOffset)
        {
            TimezoneOffset = timezoneOffset;
        }

        public int TimezoneOffset { get; }
    }
}