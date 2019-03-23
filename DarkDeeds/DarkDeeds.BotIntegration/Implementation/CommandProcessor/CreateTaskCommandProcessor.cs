using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Interface.CommandProcessor;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.BotIntegration.Implementation.CommandProcessor
{
    // TODO: unit-tests
    public class CreateTaskCommandProcessor : BaseCommandProcessor<CreateTaskCommand>, ICreateTaskCommandProcessor 
    {
        private readonly IBotSendMessageService _botSendMessageService;
        private readonly ITelegramService _telegramService;
        private readonly ITaskService _taskService;

        public CreateTaskCommandProcessor(IBotSendMessageService botSendMessageService,
            ITelegramService telegramService, ITaskService taskService) : base(botSendMessageService)
        {
            _botSendMessageService = botSendMessageService;
            _telegramService = telegramService;
            _taskService = taskService;
        }

        protected override async Task ProcessCoreAsync(CreateTaskCommand command)
        {
            string userId = await _telegramService.GetUserId(command.UserChatId);
            await _taskService.SaveTasksAsync(new[] {command.Task}, userId);
            await _botSendMessageService.SendTextAsync(command.UserChatId, "Task created");
        }
    }
}