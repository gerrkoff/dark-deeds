using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DarkDeeds.Models;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.Services.Implementation
{
    public class TaskParserService : ITaskParserService
    {
        // TODO: implement this
        // TODO: unit-tests
        public TaskDto ParseTask(string task, int timeAdjustment = 0)
        {
            var taskDto = new TaskDto();
            
            var dateRx = new Regex(@"^\d{4}\s");
            if (dateRx.IsMatch(task))
            {
                string date = task.Substring(0, 4);
                task = task.Substring(5);
                
                int month = int.Parse(date.Substring(0, 2));
                int day = int.Parse(date.Substring(2, 2));
                taskDto.DateTime = CreateDateTime(DateTime.UtcNow.Year, month, day, 0, 0, timeAdjustment);
            }

            taskDto.Title = task;
            return taskDto;
        }

        private DateTime CreateDateTime(int year, int month, int day, int hour, int minutes, int timeAdjustment)
        {
            var dateTime = new DateTime(year, month, day, hour, minutes, 0);
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            dateTime = dateTime.AddMinutes(timeAdjustment);
            return dateTime;
        }

        public string PrintTasks(IEnumerable<TaskDto> tasks, int timeAdjustment = 0)
        {
            var sb = new StringBuilder();
            foreach (var task in tasks)
            {
                if (sb.Length > 0)
                    sb.AppendLine();
                sb.Append(TaskToString(task));
            }

            return sb.ToString();
        }

        private string TaskToString(TaskDto task) => $"{task.Title}";
    }
}