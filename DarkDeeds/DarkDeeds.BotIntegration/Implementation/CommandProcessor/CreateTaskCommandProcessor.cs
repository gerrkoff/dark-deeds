using System;
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
    public class CreateTaskCommandProcessor : BaseCommandProcessor<CreateTaskCommand>, ICreateTaskCommandProcessor 
    {
        private readonly IBotSendMessageService _botSendMessageService;
        private readonly ITelegramService _telegramService;
        private readonly ITaskService _taskService;

        private Action<IEnumerable<TaskDto>> _sendUpdateTasks;

        public CreateTaskCommandProcessor(
            IBotSendMessageService botSendMessageService,
            ITelegramService telegramService,
            ITaskService taskService,
            ILogger<BaseCommandProcessor<BotCommand>> logger) : base(botSendMessageService, logger)
        {
            _botSendMessageService = botSendMessageService;
            _telegramService = telegramService;
            _taskService = taskService;
        }

        protected override async Task ProcessCoreAsync(CreateTaskCommand command)
        {
            string userId = await _telegramService.GetUserId(command.UserChatId);
            IEnumerable<TaskDto> updatedTasks = await _taskService.SaveTasksAsync(new[] {command.Task}, userId);
            _sendUpdateTasks?.Invoke(updatedTasks);
            await _botSendMessageService.SendTextAsync(command.UserChatId, "Task created");
        }

        public void BindSendUpdateTasks(Action<IEnumerable<TaskDto>> sendUpdateTasks)
        {
            _sendUpdateTasks = sendUpdateTasks;
        }
    }
}