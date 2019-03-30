using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DarkDeeds.Enums;
using DarkDeeds.Models;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.Services.Implementation
{
    public class TaskParserService : ITaskParserService
    {
        public TaskDto ParseTask(string task, int timeAdjustment = 0)
        {
            var taskDto = new TaskDto();
            task = ParseDate(task, out int year, out int month, out int day, out bool withDate);
            task = ParseTime(task, out int hour, out int minutes, out TaskTimeTypeEnum timeType);
            
            taskDto.Title = task;
            taskDto.TimeType = timeType;
            if (withDate)
                taskDto.DateTime = CreateDateTime(year, month, day, hour, minutes, timeAdjustment);
            return taskDto;
        }

        private string ParseTime(string task, out int hour, out int minutes, out TaskTimeTypeEnum timeType)
        {
            var timeAfterRx = new Regex(@"^>\d{4}\s");
            var timeRx = new Regex(@"^\d{4}\s");
            string time = string.Empty;
            hour = 0;
            minutes = 0;
            timeType = TaskTimeTypeEnum.NoTime;
            
            if (timeAfterRx.IsMatch(task))
            {
                time = task.Substring(1, 4);
                task = task.Substring(6);
                timeType = TaskTimeTypeEnum.AfterTime;
            }
            else if (timeRx.IsMatch(task))
            {
                time = task.Substring(0, 4);
                task = task.Substring(5);
                timeType = TaskTimeTypeEnum.ConcreteTime;
            }

            if (!string.IsNullOrEmpty(time))
            {
                hour = int.Parse(time.Substring(0, 2));
                minutes = int.Parse(time.Substring(2, 2));
            }

            return task;
        }

        private string ParseDate(string task, out int year, out int month, out int day, out bool withDate)
        {
            var dateWithYearRx = new Regex(@"^\d{8}\s");
            var dateRx = new Regex(@"^\d{4}\s");
            string date = string.Empty;
            year = 0;
            month = 0;
            day = 0;
            withDate = false;
                        
            if (dateWithYearRx.IsMatch(task))
            {
                date = task.Substring(4, 4);
                year = int.Parse(task.Substring(0, 4));
                task = task.Substring(9);

            }
            else if (dateRx.IsMatch(task))
            {
                date = task.Substring(0, 4);
                task = task.Substring(5);
                year = DateTime.UtcNow.Year;
            }
            
            if (!string.IsNullOrEmpty(date))
            {
                month = int.Parse(date.Substring(0, 2));
                day = int.Parse(date.Substring(2, 2));
                withDate = true;
            }
            
            return task;
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