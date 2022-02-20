using System;
using System.Collections.Generic;
using DarkDeeds.ServiceTask.Dto;

namespace DarkDeeds.ServiceTask.Services.DtoExtensions
{
    public static class RecurrenceExtensions
    {
        public static ICollection<PlannedRecurrenceDto> ToUtcDate(this ICollection<PlannedRecurrenceDto> recurrences)
        {
            foreach (var recurrence in recurrences)
            {
                recurrence.StartDate = DateTime.SpecifyKind(recurrence.StartDate, DateTimeKind.Utc);
                if (recurrence.EndDate != null)
                    recurrence.EndDate = DateTime.SpecifyKind(recurrence.EndDate.Value, DateTimeKind.Utc);
            }

            return recurrences;
        }
    }
}
