using DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp.Dto;

namespace DarkDeeds.TelegramClientApp.Services.Models.Commands
{
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
}