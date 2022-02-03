using System;

namespace DarkDeeds.TelegramClient.Services.Interface
{
    public interface IDateService
    {
        DateTime Today { get; }
        
        DateTime Now { get; }
    }
}