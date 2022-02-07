using System.Threading.Tasks;
using DarkDeeds.TelegramClient.Services.Models.Commands;

namespace DarkDeeds.TelegramClient.Services.Interface.CommandProcessor
{
    public interface IStartCommandProcessor
    {
        Task ProcessAsync(StartCommand command);
    }
}