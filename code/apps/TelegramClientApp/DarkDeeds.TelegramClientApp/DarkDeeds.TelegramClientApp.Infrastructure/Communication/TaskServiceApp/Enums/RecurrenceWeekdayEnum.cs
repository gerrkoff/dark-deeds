using System;

namespace DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp.Enums
{
    [Flags]
    public enum RecurrenceWeekdayEnum
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64,
    }
}