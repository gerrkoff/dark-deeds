using System;
using System.Collections.Generic;
using DarkDeeds.ServiceTask.Dto;

namespace DarkDeeds.ServiceTask.Services.DtoExtensions
{
    public static class TaskExtensions
    {
        public static ICollection<TaskDto> ToUtcDate(this ICollection<TaskDto> tasks)
        {
            foreach (var task in tasks)
            {
                if (task.Date != null)
                    task.Date = DateTime.SpecifyKind(task.Date.Value, DateTimeKind.Utc);
            }

            return tasks;
        }
    }
}
