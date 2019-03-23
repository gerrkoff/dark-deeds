using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Objects.Commands;

namespace DarkDeeds.BotIntegration.Implementation.CommandProcessor
{
    // TODO: unit-tests
    public abstract class BaseCommandProcessor<T> where T : BotCommand
    {
        private readonly IBotSendMessageService _botSendMessageService;

        protected BaseCommandProcessor(IBotSendMessageService botSendMessageService)
        {
            _botSendMessageService = botSendMessageService;
        }

        public async Task ProcessAsync(T command)
        {
            try
            {
                await ProcessCoreAsync(command);
            }
            catch
            {
                // TODO: log
                await _botSendMessageService.SendFailedAsync(command.UserChatId);
            }
        }

        protected abstract Task ProcessCoreAsync(T command);
    }
}