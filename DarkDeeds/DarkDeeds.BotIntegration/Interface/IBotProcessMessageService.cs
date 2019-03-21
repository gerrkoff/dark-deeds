using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Dto;

namespace DarkDeeds.BotIntegration.Interface
{
    public interface IBotProcessMessageService
    {
        Task ProcessMessageAsync(UpdateDto update);
    }
}