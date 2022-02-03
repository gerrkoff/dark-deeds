using System;
using DarkDeeds.ServiceTask.Services.Interface;

namespace DarkDeeds.ServiceTask.Services.Implementation
{
    public class DateService : IDateService
    {
        public DateTime Today => DateTime.UtcNow.Date;
        public DateTime Now => DateTime.UtcNow;
    }
}