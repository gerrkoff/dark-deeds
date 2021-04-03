using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Services.Interface;
using DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp.Dto;
using DarkDeeds.TelegramClientApp.Infrastructure.Services;
using DarkDeeds.TelegramClientApp.Services.Interface;
using DarkDeeds.TelegramClientApp.Services.Interface.CommandProcessor;
using DarkDeeds.TelegramClientApp.Services.Models.Commands;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.TelegramClientApp.Services.Implementation.CommandProcessor
{
    public class CreateTaskCommandProcessor : BaseCommandProcessor<CreateTaskCommand>, ICreateTaskCommandProcessor 
    {
        private readonly IBotSendMessageService _botSendMessageService;
        private readonly ITelegramService _telegramService;
        private readonly ITaskServiceApp _taskServiceApp;
        private readonly ITaskHubService _taskHubService;

        public CreateTaskCommandProcessor(
            IBotSendMessageService botSendMessageService,
            ITelegramService telegramService,
            ITaskServiceApp taskServiceApp,
            ITaskHubService taskHubService,
            ILogger<BaseCommandProcessor<BotCommand>> logger) : base(botSendMessageService, logger)
        {
            _botSendMessageService = botSendMessageService;
            _telegramService = telegramService;
            _taskServiceApp = taskServiceApp;
            _taskHubService = taskHubService;
        }

        protected override async Task ProcessCoreAsync(CreateTaskCommand command)
        {
            string userId = await _telegramService.GetUserId(command.UserChatId);
            IEnumerable<TaskDto> updatedTasks = await _taskServiceApp.SaveTasksAsync(new[] {command.Task}, userId);
            await _botSendMessageService.SendTextAsync(command.UserChatId, "Task created");
            await _taskHubService.Update(updatedTasks);
        }
    }
}