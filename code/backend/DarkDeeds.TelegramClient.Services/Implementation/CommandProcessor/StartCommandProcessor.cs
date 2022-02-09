using System.Threading.Tasks;
using DarkDeeds.TelegramClient.Services.Interface;
using DarkDeeds.TelegramClient.Services.Interface.CommandProcessor;
using DarkDeeds.TelegramClient.Services.Models.Commands;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.TelegramClient.Services.Implementation.CommandProcessor
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