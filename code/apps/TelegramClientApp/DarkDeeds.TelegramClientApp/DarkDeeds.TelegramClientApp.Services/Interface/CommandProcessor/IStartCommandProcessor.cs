using System.Threading.Tasks;
using DarkDeeds.TelegramClientApp.Services.Models.Commands;

namespace DarkDeeds.TelegramClientApp.Services.Interface.CommandProcessor
{
    public interface IStartCommandProcessor
    {
        Task ProcessAsync(StartCommand command);
    }
}