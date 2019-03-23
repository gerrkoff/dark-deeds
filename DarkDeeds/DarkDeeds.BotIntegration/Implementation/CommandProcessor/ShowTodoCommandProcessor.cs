using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Interface.CommandProcessor;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.BotIntegration.Implementation.CommandProcessor
{
    public class ShowTodoCommandProcessor : BaseCommandProcessor<ShowTodoCommand>, IShowTodoCommandProcessor
    {
        private readonly IBotSendMessageService _botSendMessageService;
        private readonly ITelegramService _telegramService;

        public ShowTodoCommandProcessor(IBotSendMessageService botSendMessageService, ITelegramService telegramService)
            : base(botSendMessageService)
        {
            _botSendMessageService = botSendMessageService;
            _telegramService = telegramService;
        }

        protected override async Task ProcessCoreAsync(ShowTodoCommand command)
        {
            string userId = await _telegramService.GetUserId(command.UserChatId);
            
            await _botSendMessageService.SendTextAsync(command.UserChatId, $"Show todo {command.Day}");
        }
    }
}