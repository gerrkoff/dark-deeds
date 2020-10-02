using DarkDeeds.Models.Dto;

namespace DarkDeeds.BotIntegration.Objects.Commands
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