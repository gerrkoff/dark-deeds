using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Objects.Commands;

namespace DarkDeeds.BotIntegration.Interface
{
    public interface IBotProcessShowTodoService
    {
        Task ProcessAsync(ShowTodoCommand command);
    }
}