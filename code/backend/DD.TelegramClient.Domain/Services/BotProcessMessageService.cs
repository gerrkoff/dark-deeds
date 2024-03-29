using DD.TelegramClient.Domain.Dto;
using DD.TelegramClient.Domain.Models.Commands;
using DD.TelegramClient.Domain.Services.CommandProcessor;

namespace DD.TelegramClient.Domain.Services;

public interface IBotProcessMessageService
{
    Task ProcessMessageAsync(UpdateDto update);
}

public class BotProcessMessageService(
    IBotSendMessageService botSendMessageService,
    IBotCommandParserService botCommandParserService,
    IShowTodoCommandProcessor showTodoCommandProcessor,
    ICreateTaskCommandProcessor createTaskCommandProcessor,
    IStartCommandProcessor startCommandProcessor)
    : IBotProcessMessageService
{
    public async Task ProcessMessageAsync(UpdateDto update)
    {
        var text = update.Message?.Text?.Trim() ?? string.Empty;
        var userChatId = update.Message?.Chat?.Id ?? throw new InvalidOperationException("Chat id is not found");

        var command = await botCommandParserService.ParseCommand(text, userChatId);
        if (command != null)
            command.UserChatId = userChatId;

        if (command is ShowTodoCommand showTodoCommand)
        {
            await showTodoCommandProcessor.ProcessAsync(showTodoCommand);
            return;
        }

        if (command is StartCommand startCommand)
        {
            await startCommandProcessor.ProcessAsync(startCommand);
            return;
        }

        if (command is CreateTaskCommand createTaskCommand)
        {
            await createTaskCommandProcessor.ProcessAsync(createTaskCommand);
            return;
        }

        await botSendMessageService.SendUnknownCommandAsync(userChatId);
    }
}
