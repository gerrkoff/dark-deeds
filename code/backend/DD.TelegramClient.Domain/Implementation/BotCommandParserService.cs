using DD.TelegramClient.Domain.Infrastructure;
using DD.TelegramClient.Domain.Models.Commands;

namespace DD.TelegramClient.Domain.Implementation;

public interface IBotCommandParserService
{
    Task<BotCommand> ParseCommand(string command, int chatId);
}

public class BotCommandParserService(
    ITelegramService telegramService,
    IDateService dateService,
    ITaskServiceApp taskServiceApp)
    : IBotCommandParserService
{
    const string TodoCommand = "/todo";
    const string StartCommand = "/start";

    public async Task<BotCommand> ParseCommand(string command, int chatId)
    {
        if (CheckAndTrimCommand(StartCommand, command, out var args))
            return new StartCommand(args);

        if (CheckAndTrimCommand(TodoCommand, command, out args))
        {
            int timeAdjustment = await telegramService.GetUserTimeAdjustment(chatId);
            DateTime now = dateService.Now;
            return new ShowTodoCommand(args, now, timeAdjustment);
        }

        if (!command.StartsWith("/"))
            return new CreateTaskCommand(await taskServiceApp.ParseTask(command));

        return null;
    }

    private bool CheckAndTrimCommand(string command, string text, out string args)
    {
        args = string.Empty;

        if (string.Equals(text, command))
            return true;

        if (text.StartsWith(command + " "))
        {
            args = text.Substring(command.Length + 1).Trim();
            return true;
        }

        return false;
    }
}
