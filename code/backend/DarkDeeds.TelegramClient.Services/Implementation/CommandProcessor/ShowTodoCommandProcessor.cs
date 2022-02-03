using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.TelegramClient.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.TelegramClient.Infrastructure.Communication.TaskServiceApp.Dto;
using DarkDeeds.TelegramClient.Services.Interface;
using DarkDeeds.TelegramClient.Services.Interface.CommandProcessor;
using DarkDeeds.TelegramClient.Services.Models.Commands;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.TelegramClient.Services.Implementation.CommandProcessor
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
            ICollection<string> tasksAsString = await _taskServiceApp.PrintTasks(tasks);
            string text = tasksAsString.Count == 0
                ? "No tasks"
                : string.Join("\n", tasksAsString);
            await _botSendMessageService.SendTextAsync(command.UserChatId, text);
        }
    }
}