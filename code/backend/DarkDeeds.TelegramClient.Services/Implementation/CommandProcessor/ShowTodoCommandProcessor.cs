using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Consumers;
using DarkDeeds.TelegramClient.Services.Interface;
using DarkDeeds.TelegramClient.Services.Interface.CommandProcessor;
using DarkDeeds.TelegramClient.Services.Models.Commands;
using DD.TaskService.Domain.Dto;
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
            IEnumerable<TaskDto> tasks = await _taskServiceApp.LoadTasksByDateAsync(command.From, command.To, userId);
            ICollection<string> tasksAsString = await _taskServiceApp.PrintTasks(tasks);
            string text = tasksAsString.Count == 0
                ? "No tasks"
                : string.Join("\n", tasksAsString);
            await _botSendMessageService.SendTextAsync(command.UserChatId, text);
        }
    }
}
