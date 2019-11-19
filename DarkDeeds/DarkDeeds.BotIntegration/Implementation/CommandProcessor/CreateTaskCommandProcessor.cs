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
        private readonly ITaskHubService _taskHubService;

        public CreateTaskCommandProcessor(
            IBotSendMessageService botSendMessageService,
            ITelegramService telegramService,
            ITaskService taskService,
            ITaskHubService taskHubService,
            ILogger<BaseCommandProcessor<BotCommand>> logger) : base(botSendMessageService, logger)
        {
            _botSendMessageService = botSendMessageService;
            _telegramService = telegramService;
            _taskService = taskService;
            _taskHubService = taskHubService;
        }

        protected override async Task ProcessCoreAsync(CreateTaskCommand command)
        {
            string userId = await _telegramService.GetUserId(command.UserChatId);
            IEnumerable<TaskDto> updatedTasks = await _taskService.SaveTasksAsync(new[] {command.Task}, userId);
            await _botSendMessageService.SendTextAsync(command.UserChatId, "Task created");
            await _taskHubService.Update(updatedTasks);
        }
    }
}