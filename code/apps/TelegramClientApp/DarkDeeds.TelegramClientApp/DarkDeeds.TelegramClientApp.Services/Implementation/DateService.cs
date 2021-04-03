using System;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.TelegramClientApp.Services.Implementation
{
    public class DateService : IDateService
    {
        public DateTime Today => DateTime.UtcNow.Date;
        public DateTime Now => DateTime.UtcNow;
    }
}