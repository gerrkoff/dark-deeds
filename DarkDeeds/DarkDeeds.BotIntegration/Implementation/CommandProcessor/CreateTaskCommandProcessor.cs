using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Interface.CommandProcessor;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.BotIntegration.Implementation.CommandProcessor
{
    public class CreateTaskCommandProcessor : BaseCommandProcessor<CreateTaskCommand>, ICreateTaskCommandProcessor 
    {
        private readonly IBotSendMessageService _botSendMessageService;
        private readonly ITelegramService _telegramService;

        public CreateTaskCommandProcessor(IBotSendMessageService botSendMessageService,
            ITelegramService telegramService) : base(botSendMessageService)
        {
            _botSendMessageService = botSendMessageService;
            _telegramService = telegramService;
        }

        protected override async Task ProcessCoreAsync(CreateTaskCommand command)
        {
            string userId = await _telegramService.GetUserId(command.UserChatId);
            await _botSendMessageService.SendTextAsync(command.UserChatId, $"Create task: {command.Task.Title}");
        }
    }
}