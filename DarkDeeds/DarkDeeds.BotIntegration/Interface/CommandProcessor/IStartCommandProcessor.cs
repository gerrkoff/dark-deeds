using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Objects.Commands;

namespace DarkDeeds.BotIntegration.Interface.CommandProcessor
{
    public interface IStartCommandProcessor
    {
        Task ProcessAsync(StartCommand command);
    }
}