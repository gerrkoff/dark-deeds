namespace DarkDeeds.BotIntegration.Objects.Commands
{
    public class CreateTaskCommand
    {
        public string Task { get; private set; }

        public CreateTaskCommand(string args)
        {
            Task = args;
        }
    }
}