using System.Threading.Tasks;
using DarkDeeds.TelegramClient.Services.Models.Commands;

namespace DarkDeeds.TelegramClient.Services.Interface.CommandProcessor
{
    public interface IShowTodoCommandProcessor
    {
        Task ProcessAsync(ShowTodoCommand command);
    }
}