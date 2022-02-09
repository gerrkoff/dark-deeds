using System;
using DarkDeeds.TelegramClient.Services.Interface;

namespace DarkDeeds.TelegramClient.Services.Implementation
{
    class DateService : IDateService
    {
        public DateTime Today => DateTime.UtcNow.Date;
        public DateTime Now => DateTime.UtcNow;
    }
}