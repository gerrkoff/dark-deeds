using DarkDeeds.Models.Bot;

namespace DarkDeeds.Services.Interface
{
    public interface IBotProcessMessageService
    {
        void ProcessMessage(UpdateDto update);
    }
}