using System;
using DarkDeeds.TaskServiceApp.Services.Interface;

namespace DarkDeeds.TaskServiceApp.Services.Implementation
{
    public class DateService : IDateService
    {
        public DateTime Today => DateTime.UtcNow.Date;
        public DateTime Now => DateTime.UtcNow;
    }
}