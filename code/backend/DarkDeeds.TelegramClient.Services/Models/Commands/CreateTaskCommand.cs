using DD.TaskService.Domain.Dto;

namespace DarkDeeds.TelegramClient.Services.Models.Commands;

public class CreateTaskCommand : BotCommand
{
    public TaskDto Task { get; }

    public CreateTaskCommand(TaskDto task)
    {
        Task = task;
    }

    public override string ToString()
    {
        return $"{nameof(CreateTaskCommand)} {base.ToString()}";
    }
}
