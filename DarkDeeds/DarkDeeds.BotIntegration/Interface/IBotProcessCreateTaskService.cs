using DarkDeeds.BotIntegration.Objects.Commands;

namespace DarkDeeds.BotIntegration.Interface
{
    public interface IBotProcessCreateTaskService
    {
        void Process(CreateTaskCommand command);
    }
}