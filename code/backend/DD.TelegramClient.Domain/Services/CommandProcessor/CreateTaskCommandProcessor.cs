using DD.Shared.Details.Abstractions;
using DD.TelegramClient.Domain.Models.Commands;
using Microsoft.Extensions.Logging;

namespace DD.TelegramClient.Domain.Services.CommandProcessor;

public interface ICreateTaskCommandProcessor
{
    Task ProcessAsync(CreateTaskCommand command);
}

public class CreateTaskCommandProcessor(
    IBotSendMessageService botSendMessageService,
    ITelegramService telegramService,
    ITaskServiceApp taskServiceApp,
    ILogger<BaseCommandProcessor<BotCommand>> logger)
    : BaseCommandProcessor<CreateTaskCommand>(botSendMessageService, logger), ICreateTaskCommandProcessor
{
    private readonly IBotSendMessageService _botSendMessageService = botSendMessageService;

    protected override async Task ProcessCoreAsync(CreateTaskCommand command)
    {
        var userId = await telegramService.GetUserId(command.UserChatId);
        var tasks = await taskServiceApp.ParseTasks(command.Text);

        foreach (var task in tasks)
            task.Uid = Guid.NewGuid().ToString();

        await taskServiceApp.SaveTasksAsync([.. tasks], userId, clientId: null);
        await _botSendMessageService.SendTextAsync(
            command.UserChatId,
            tasks.Count == 1 ? "Task created" : $"{tasks.Count} tasks created");
    }
}
