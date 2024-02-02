using DD.TelegramClient.Domain.Infrastructure.Dto;

namespace DD.TelegramClient.Domain.Models.Commands;

public class CreateTaskCommand(TaskDto task) : BotCommand
{
    public TaskDto Task { get; } = task;

    public override string ToString()
    {
        return $"{nameof(CreateTaskCommand)} {base.ToString()}";
    }
}
