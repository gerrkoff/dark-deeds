using System.Threading.Tasks;
using DarkDeeds.TelegramClientApp.Services.Interface;
using DarkDeeds.TelegramClientApp.Services.Models.Commands;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.TelegramClientApp.Services.Implementation.CommandProcessor
{
    public abstract class BaseCommandProcessor<T> where T : BotCommand
    {
        private readonly IBotSendMessageService _botSendMessageService;
        private readonly ILogger<BaseCommandProcessor<BotCommand>> _logger;

        protected BaseCommandProcessor(IBotSendMessageService botSendMessageService,
            ILogger<BaseCommandProcessor<BotCommand>> logger)
        {
            _botSendMessageService = botSendMessageService;
            _logger = logger;
        }

        public async Task ProcessAsync(T command)
        {
            try
            {
                await ProcessCoreAsync(command);
            }
            catch
            {
                _logger.LogWarning("Command processing failed. Command: " + command);
                await _botSendMessageService.SendFailedAsync(command.UserChatId);
            }
        }

        protected abstract Task ProcessCoreAsync(T command);
    }
}