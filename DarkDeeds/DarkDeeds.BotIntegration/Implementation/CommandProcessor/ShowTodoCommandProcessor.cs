using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Interface.CommandProcessor;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Infrastructure.Communication;
using DarkDeeds.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.Infrastructure.Communication.TaskServiceApp.Dto;
using DarkDeeds.Models.Dto;
using DarkDeeds.Services.Interface;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.BotIntegration.Implementation.CommandProcessor
{
    public class ShowTodoCommandProcessor : BaseCommandProcessor<ShowTodoCommand>, IShowTodoCommandProcessor
    {
        private readonly IBotSendMessageService _botSendMessageService;
        private readonly ITelegramService _telegramService;
        private readonly ITaskServiceApp _taskServiceApp;

        public ShowTodoCommandProcessor(
            IBotSendMessageService botSendMessageService,
            ITelegramService telegramService,
            ILogger<BaseCommandProcessor<BotCommand>> logger,
            ITaskServiceApp taskServiceApp) : base(botSendMessageService, logger)
        {
            _botSendMessageService = botSendMessageService;
            _telegramService = telegramService;
            _taskServiceApp = taskServiceApp;
        }

        protected override async Task ProcessCoreAsync(ShowTodoCommand command)
        {
            string userId = await _telegramService.GetUserId(command.UserChatId);
            IEnumerable<TaskDto> tasks = await _taskServiceApp.LoadTasksByDateAsync(userId, command.From, command.To);
            string tasksAsString = await _taskServiceApp.PrintTasks(tasks);
            if (string.IsNullOrEmpty(tasksAsString))
                tasksAsString = "No tasks";
            await _botSendMessageService.SendTextAsync(command.UserChatId, tasksAsString);
        }
    }
}