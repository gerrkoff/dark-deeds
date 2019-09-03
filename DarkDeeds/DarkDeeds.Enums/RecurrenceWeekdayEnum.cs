using System;

namespace DarkDeeds.Enums
{
    [Flags]
    public enum RecurrenceWeekdayEnum
    {
        None = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64,
    }
}