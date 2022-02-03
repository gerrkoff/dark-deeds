using System.Threading.Tasks;
using DarkDeeds.TelegramClient.Services.Dto;

namespace DarkDeeds.TelegramClient.Services.Interface
{
    public interface IBotProcessMessageService
    {
        Task ProcessMessageAsync(UpdateDto update);
    }
}