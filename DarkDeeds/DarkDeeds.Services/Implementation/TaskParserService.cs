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
        private readonly IDateService _dateService;

        public TaskParserService(IDateService dateService)
        {
            _dateService = dateService;
        }

        // TODO: refactor
        public TaskDto ParseTask(string task, int timeAdjustment = 0)
        {
            var taskDto = new TaskDto();
            var timeType = TaskTimeTypeEnum.NoTime;
            task = ParseDate(task, out int year, out int month, out int day, out bool withDate, ref timeType, out int dayAdjustment);
            task = ParseTime(task, out int hour, out int minutes, ref timeType);
            task = ParseProbability(task, out bool isProbable);
            
            taskDto.Title = task;
            taskDto.TimeType = timeType;
            taskDto.IsProbable = isProbable;
            if (withDate)
                taskDto.DateTime = CreateDateTime(year, month, day, hour, minutes, timeAdjustment, dayAdjustment);
            return taskDto;
        }

        private string ParseProbability(string task, out bool isProbable)
        {
            isProbable = false;
            if (task.EndsWith(" ?"))
            {
                task = task.Substring(0, task.Length - 2);
                isProbable = true;
            }

            return task;
        }

        private string ParseTime(string task, out int hour, out int minutes, ref TaskTimeTypeEnum timeType)
        {
            var timeRx = new Regex(@"^\d{4}\s");
            string time = string.Empty;
            hour = 0;
            minutes = 0;

            if (timeType == TaskTimeTypeEnum.AllDayLong)
                return task;

            if (timeRx.IsMatch(task))
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

        private string ParseDate(string task, out int year, out int month, out int day, out bool withDate, ref TaskTimeTypeEnum timeType, out int dayAdjustment)
        {
            var dateWithYearRx = new Regex(@"^\d{8}!?\s");
            var dateRx = new Regex(@"^\d{4}!?\s");
            var todayShiftRx = new Regex(@"^!+\s");
            var weekShiftRx = new Regex(@"^![1-7]\s");
            string date = string.Empty;
            year = 0;
            month = 0;
            day = 0;
            dayAdjustment = 0;
            withDate = false;
                        
            if (dateWithYearRx.IsMatch(task))
            {
                date = task.Substring(4, 4);
                year = int.Parse(task.Substring(0, 4));
                task = task.Substring(8);
                task = ParseAllDayLong(task, ref timeType);
                task = task.Substring(1);
            }
            else if (dateRx.IsMatch(task))
            {
                date = task.Substring(0, 4);
                year = _dateService.Today.Year;
                task = task.Substring(4);
                task = ParseAllDayLong(task, ref timeType);
                task = task.Substring(1);
            }
            else if (todayShiftRx.IsMatch(task))
            {
                task = ParseTodayShift(task, out dayAdjustment);
                year = _dateService.Today.Year;
                month = _dateService.Today.Month;
                day = _dateService.Today.Day;
                withDate = true;
            }
            else if (weekShiftRx.IsMatch(task))
            {
                task = ParseWeekShift(task, out dayAdjustment);
                year = _dateService.Today.Year;
                month = _dateService.Today.Month;
                day = _dateService.Today.Day;
                withDate = true;
            }
            
            if (!string.IsNullOrEmpty(date))
            {
                month = int.Parse(date.Substring(0, 2));
                day = int.Parse(date.Substring(2, 2));
                withDate = true;
            }
            
            return task;
        }

        private string ParseWeekShift(string task, out int dayAdjustment)
        {
            int dayShift = int.Parse(task[1].ToString());
            int nextSundayShift = (7 - (int) _dateService.Today.DayOfWeek) % 7;
            dayAdjustment = nextSundayShift + dayShift;
            return task.Substring(3);
        }

        private string ParseTodayShift(string task, out int dayAdjustment)
        {
            dayAdjustment = new Regex("!+").Matches(task)[0].Length;
            dayAdjustment--;
            return task.Substring(dayAdjustment + 2);
        }

        private string ParseAllDayLong(string task, ref TaskTimeTypeEnum timeType)
        {
            if (task.StartsWith("!"))
            {
                task = task.Substring(1);
                timeType = TaskTimeTypeEnum.AllDayLong;
            }

            return task;
        }

        private DateTime CreateDateTime(int year, int month, int day, int hour, int minutes, int timeAdjustment, int dayAdjustment)
        {
            var dateTime = new DateTime(year, month, day, hour, minutes, 0);
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            dateTime = dateTime.AddMinutes(timeAdjustment).AddDays(dayAdjustment);
            return dateTime;
        }

        public string PrintTasks(IEnumerable<TaskDto> tasks, int timeAdjustment = 0)
        {
            var sb = new StringBuilder();
            foreach (var task in tasks)
            {
                if (sb.Length > 0)
                    sb.AppendLine();
                sb.Append(TaskToString(task, timeAdjustment));
            }

            return sb.ToString();
        }

        // TODO: sync with FE
        private string TaskToString(TaskDto task, int timeAdjustment)
        {
            string result = string.Empty;
            if (task.DateTime.HasValue)
            {
                task.DateTime = task.DateTime.Value.AddMinutes(timeAdjustment);
                if (task.TimeType == TaskTimeTypeEnum.ConcreteTime)
                    result += $"{DateToTimeString(task.DateTime.Value)} ";
            }

            result += task.Title;

            return result;
        }

        private string DateToTimeString(DateTime dateTime) => $"{dateTime.Hour:D2}:{dateTime.Minute:D2}";
    }
}