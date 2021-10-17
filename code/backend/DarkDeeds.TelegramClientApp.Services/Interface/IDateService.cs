using System;

namespace DarkDeeds.TelegramClientApp.Services.Interface
{
    public interface IDateService
    {
        DateTime Today { get; }
        
        DateTime Now { get; }
    }
}