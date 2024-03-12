using System.Diagnostics.CodeAnalysis;
using DD.TelegramClient.Domain.Models.Commands;
using Microsoft.Extensions.Logging;

namespace DD.TelegramClient.Domain.Services.CommandProcessor;

public abstract class BaseCommandProcessor<T>(
    IBotSendMessageService botSendMessageService,
    ILogger<BaseCommandProcessor<BotCommand>> logger)
    where T : BotCommand
{
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "We need to catch all exceptions to log them and send a message to the user.")]
    public async Task ProcessAsync(T command)
    {
        try
        {
            await ProcessCoreAsync(command);
        }
        catch
        {
            Log.FailedToProcessCommand(logger, command);
            await botSendMessageService.SendFailedAsync(command.UserChatId);
        }
    }

    protected abstract Task ProcessCoreAsync(T command);
}
