using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Interface.CommandProcessor;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Models;
using DarkDeeds.Services.Interface;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.BotIntegration.Implementation.CommandProcessor
{
    public class ShowTodoCommandProcessor : BaseCommandProcessor<ShowTodoCommand>, IShowTodoCommandProcessor
    {
        private readonly IBotSendMessageService _botSendMessageService;
        private readonly ITelegramService _telegramService;
        private readonly ITaskService _taskService;
        private readonly ITaskParserService _taskParserService;

        public ShowTodoCommandProcessor(
            IBotSendMessageService botSendMessageService,
            ITelegramService telegramService,
            ITaskService taskService,
            ITaskParserService taskParserService,
            ILogger<BaseCommandProcessor<BotCommand>> logger) : base(botSendMessageService, logger)
        {
            _botSendMessageService = botSendMessageService;
            _telegramService = telegramService;
            _taskService = taskService;
            _taskParserService = taskParserService;
        }

        protected override async Task ProcessCoreAsync(ShowTodoCommand command)
        {
            string userId = await _telegramService.GetUserId(command.UserChatId);
            IEnumerable<TaskDto> tasks = await _taskService.LoadTasksByDateAsync(userId, command.From, command.To);
            string tasksAsString = _taskParserService.PrintTasks(tasks);
            if (string.IsNullOrEmpty(tasksAsString))
                tasksAsString = "No tasks";
            await _botSendMessageService.SendTextAsync(command.UserChatId, tasksAsString);
        }
    }
}