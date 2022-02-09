using System;
using System.Threading.Tasks;
using DarkDeeds.TelegramClient.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.TelegramClient.Services.Interface;
using DarkDeeds.TelegramClient.Services.Interface.CommandProcessor;
using DarkDeeds.TelegramClient.Services.Models.Commands;
using Microsoft.Extensions.Logging;

namespace DarkDeeds.TelegramClient.Services.Implementation.CommandProcessor
{
    public class CreateTaskCommandProcessor : BaseCommandProcessor<CreateTaskCommand>, ICreateTaskCommandProcessor 
    {
        private readonly IBotSendMessageService _botSendMessageService;
        private readonly ITelegramService _telegramService;
        private readonly ITaskServiceApp _taskServiceApp;

        public CreateTaskCommandProcessor(
            IBotSendMessageService botSendMessageService,
            ITelegramService telegramService,
            ITaskServiceApp taskServiceApp,
            ILogger<BaseCommandProcessor<BotCommand>> logger) : base(botSendMessageService, logger)
        {
            _botSendMessageService = botSendMessageService;
            _telegramService = telegramService;
            _taskServiceApp = taskServiceApp;
        }

        protected override async Task ProcessCoreAsync(CreateTaskCommand command)
        {
            string userId = await _telegramService.GetUserId(command.UserChatId);
            command.Task.Uid = Guid.NewGuid().ToString();
            await _taskServiceApp.SaveTasksAsync(new[] {command.Task}, userId);
            await _botSendMessageService.SendTextAsync(command.UserChatId, "Task created");
        }
    }
}