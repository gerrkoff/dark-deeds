using System;
using System.Collections.Generic;
using DarkDeeds.TaskServiceApp.Models.Dto;

namespace DarkDeeds.TaskServiceApp.Models.Extensions
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