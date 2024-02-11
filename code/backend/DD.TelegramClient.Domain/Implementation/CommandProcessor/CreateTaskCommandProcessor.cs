using DD.TelegramClient.Domain.Infrastructure;
using DD.TelegramClient.Domain.Models.Commands;
using Microsoft.Extensions.Logging;

namespace DD.TelegramClient.Domain.Implementation.CommandProcessor;

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
        command.Task.Uid = Guid.NewGuid().ToString();
        await taskServiceApp.SaveTasksAsync(new[] { command.Task }, userId);
        await _botSendMessageService.SendTextAsync(command.UserChatId, "Task created");
    }
}
