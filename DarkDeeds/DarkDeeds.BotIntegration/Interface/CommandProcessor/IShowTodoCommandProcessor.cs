using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Objects.Commands;

namespace DarkDeeds.BotIntegration.Interface.CommandProcessor
{
    public interface IShowTodoCommandProcessor
    {
        Task ProcessAsync(ShowTodoCommand command);
    }
}