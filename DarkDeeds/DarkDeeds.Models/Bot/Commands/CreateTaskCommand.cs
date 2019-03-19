namespace DarkDeeds.Models.Bot.Commands
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