using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Objects.Commands;

namespace DarkDeeds.BotIntegration.Interface
{
    public interface IBotProcessCreateTaskService
    {
        Task ProcessAsync(CreateTaskCommand command);
    }
}