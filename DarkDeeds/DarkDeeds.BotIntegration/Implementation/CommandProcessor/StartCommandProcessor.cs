using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Interface.CommandProcessor;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.BotIntegration.Implementation.CommandProcessor
{
    // TODO: unit-tests
    public class StartCommandProcessor : BaseCommandProcessor<StartCommand>, IStartCommandProcessor
    {
        private readonly IBotSendMessageService _botSendMessageService;
        private readonly ITelegramService _telegramService;

        public StartCommandProcessor(IBotSendMessageService botSendMessageService, ITelegramService telegramService) :
            base(botSendMessageService)
        {
            _telegramService = telegramService;
            _botSendMessageService = botSendMessageService;
        }

        protected override async Task ProcessCoreAsync(StartCommand command)
        {
            await _telegramService.UpdateChatId(command.UserChatKey, command.UserChatId);
            await _botSendMessageService.SendTextAsync(command.UserChatId, "Registered");
        }
    }
}