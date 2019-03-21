using DarkDeeds.Models;

namespace DarkDeeds.BotIntegration.Objects.Commands
{
    public class CreateTaskCommand : BotCommand
    {
        public TaskDto Task { get; }

        public CreateTaskCommand(TaskDto task)
        {
            Task = task;
        }
    }
}