using System;
using System.Collections.Generic;
using DarkDeeds.Models;

namespace DarkDeeds.Common.Extensions
{
    public static class TaskExtensions
    {
        public static ICollection<TaskDto> ToUtcDate(this ICollection<TaskDto> tasks)
        {
            foreach (var task in tasks)
            {
                if (task.DateTime != null)
                    task.DateTime = DateTime.SpecifyKind(task.DateTime.Value, DateTimeKind.Utc);
            }

            return tasks;
        }      
    }
}