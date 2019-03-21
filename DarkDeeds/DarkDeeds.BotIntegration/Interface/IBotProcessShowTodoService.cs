using DarkDeeds.BotIntegration.Objects.Commands;

namespace DarkDeeds.BotIntegration.Interface
{
    public interface IBotProcessShowTodoService
    {
        void Process(ShowTodoCommand command);
    }
}