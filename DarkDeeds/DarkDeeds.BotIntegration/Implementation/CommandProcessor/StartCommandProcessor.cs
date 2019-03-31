using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Interface.CommandProcessor;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Services.Interface;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.BotIntegration.Implementation.CommandProcessor
{
    public class StartCommandProcessor : BaseCommandProcessor<StartCommand>, IStartCommandProcessor
    {
        private readonly IBotSendMessageService _botSendMessageService;
        private readonly ITelegramService _telegramService;

        public StartCommandProcessor(
            IBotSendMessageService botSendMessageService,
            ITelegramService telegramService,
            ILogger<BaseCommandProcessor<BotCommand>> logger) : base(botSendMessageService, logger)
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