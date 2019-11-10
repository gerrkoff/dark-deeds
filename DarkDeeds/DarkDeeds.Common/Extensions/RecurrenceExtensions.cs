using System;
using System.Collections.Generic;
using DarkDeeds.Models;

namespace DarkDeeds.Common.Extensions
{
    public static class RecurrenceExtensions
    {
        public static ICollection<PlannedRecurrenceDto> ToUtcDate(this ICollection<PlannedRecurrenceDto> tasks)
        {
            foreach (var task in tasks)
            {
                task.StartDate = DateTime.SpecifyKind(task.StartDate, DateTimeKind.Utc);
                if (task.EndDate != null)
                    task.EndDate = DateTime.SpecifyKind(task.EndDate.Value, DateTimeKind.Utc);
            }

            return tasks;
        }      
    }
}