using System;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.Services.Implementation
{
    public class DateService : IDateService
    {
        public DateTime Now => DateTime.Now;
    }
}