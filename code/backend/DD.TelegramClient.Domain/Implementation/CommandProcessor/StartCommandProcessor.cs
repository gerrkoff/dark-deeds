using DD.TelegramClient.Domain.Models.Commands;
using Microsoft.Extensions.Logging;

namespace DD.TelegramClient.Domain.Implementation.CommandProcessor;

public interface IStartCommandProcessor
{
    Task ProcessAsync(StartCommand command);
}

public class StartCommandProcessor(
    IBotSendMessageService botSendMessageService,
    ITelegramService telegramService,
    ILogger<BaseCommandProcessor<BotCommand>> logger)
    : BaseCommandProcessor<StartCommand>(botSendMessageService, logger), IStartCommandProcessor
{
    private readonly IBotSendMessageService _botSendMessageService = botSendMessageService;

    protected override async Task ProcessCoreAsync(StartCommand command)
    {
        await telegramService.UpdateChatId(command.UserChatKey, command.UserChatId);
        await _botSendMessageService.SendTextAsync(command.UserChatId, "Registered");
    }
}
