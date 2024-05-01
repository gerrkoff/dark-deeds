using DD.Shared.Details.Abstractions;
using DD.TelegramClient.Domain.Models.Commands;

namespace DD.TelegramClient.Domain.Services;

public interface IBotCommandParserService
{
    Task<BotCommand?> ParseCommand(string command, int chatId);
}

public class BotCommandParserService(
    ITelegramService telegramService,
    IDateService dateService,
    ITaskServiceApp taskServiceApp)
    : IBotCommandParserService
{
    private const string TodoCommand = "/todo";
    private const string StartCommand = "/start";

    public async Task<BotCommand?> ParseCommand(string command, int chatId)
    {
        if (CheckAndTrimCommand(StartCommand, command, out var args))
            return new StartCommand(args);

        if (CheckAndTrimCommand(TodoCommand, command, out args))
        {
            var timeAdjustment = await telegramService.GetUserTimeAdjustment(chatId);
            var now = dateService.Now;
            return new ShowTodoCommand(args, now, timeAdjustment);
        }

        return command.StartsWith('/')
            ? null
            : new CreateTaskCommand(await taskServiceApp.ParseTask(command));
    }

    private static bool CheckAndTrimCommand(string command, string text, out string args)
    {
        args = string.Empty;

        if (string.Equals(text, command, StringComparison.Ordinal))
            return true;

        if (text.StartsWith(command + " ", StringComparison.Ordinal))
        {
            args = text[(command.Length + 1)..].Trim();
            return true;
        }

        return false;
    }
}
