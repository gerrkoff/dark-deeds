using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp.Dto;
using MediatR;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Telegram.Start
{
    public class SaveRequestHandler : IRequestHandler<StartRequestModel, TelegramStartDto>
    {
        private readonly ITelegramClientApp _telegramClientApp;

        public SaveRequestHandler(ITelegramClientApp telegramClientApp)
        {
            _telegramClientApp = telegramClientApp;
        }

        public Task<TelegramStartDto> Handle(StartRequestModel request, CancellationToken cancellationToken)
        {
            return _telegramClientApp.Start(request.TimezoneOffset);
        }
    }
}