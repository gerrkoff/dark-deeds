using DD.Shared.Details.Abstractions;
using DD.TelegramClient.Domain.Models.Commands;
using Microsoft.Extensions.Logging;

namespace DD.TelegramClient.Domain.Services.CommandProcessor;

public interface IShowTodoCommandProcessor
{
    Task ProcessAsync(ShowTodoCommand command);
}

public class ShowTodoCommandProcessor(
    IBotSendMessageService botSendMessageService,
    ITelegramService telegramService,
    ILogger<BaseCommandProcessor<BotCommand>> logger,
    ITaskServiceApp taskServiceApp,
    ITaskPrinter taskPrinter)
    : BaseCommandProcessor<ShowTodoCommand>(botSendMessageService, logger), IShowTodoCommandProcessor
{
    private readonly IBotSendMessageService _botSendMessageService = botSendMessageService;

    protected override async Task ProcessCoreAsync(ShowTodoCommand command)
    {
        var userId = await telegramService.GetUserId(command.UserChatId);
        var tasks = await taskServiceApp.LoadTasksByDateAsync(command.From, command.To, userId);
        var tasksAsString = tasks.Select(taskPrinter.PrintWithSymbolCodes).ToList();
        var text = tasksAsString.Count == 0
            ? "No tasks"
            : string.Join("\n", tasksAsString);
        await _botSendMessageService.SendTextAsync(command.UserChatId, text);
    }
}
