using DD.TelegramClient.Domain.Models.Commands;
using Microsoft.Extensions.Logging;

namespace DD.TelegramClient.Domain.Implementation.CommandProcessor;

public abstract class BaseCommandProcessor<T>(
    IBotSendMessageService botSendMessageService,
    ILogger<BaseCommandProcessor<BotCommand>> logger)
    where T : BotCommand
{
    public async Task ProcessAsync(T command)
    {
        try
        {
            await ProcessCoreAsync(command);
        }
        catch
        {
            logger.LogWarning("Command processing failed. Command: " + command);
            await botSendMessageService.SendFailedAsync(command.UserChatId);
        }
    }

    protected abstract Task ProcessCoreAsync(T command);
}
