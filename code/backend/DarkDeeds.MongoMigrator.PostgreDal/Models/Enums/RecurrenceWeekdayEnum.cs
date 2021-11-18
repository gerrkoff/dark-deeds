using System;

namespace DarkDeeds.MongoMigrator.PostgreDal.Models.Enums
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