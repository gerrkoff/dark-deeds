using System;
using System.Collections.Generic;
using System.Linq;
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

        public TaskDto ParseTask(string task, bool ignoreDate = false)
        {
            var taskDto = new TaskDto();
            int year = 0, month = 0, day = 0, dayAdjustment = 0;
            bool withDate = false;
            
            if (!ignoreDate) 
                task = ParseDate(task, out year, out month, out day, out withDate, out dayAdjustment);
            task = ParseTime(task, out int hour, out int minutes, out bool withTime);
            task = ParseFlags(task, out bool isProbable, out TaskTypeEnum type);
            
            taskDto.Title = task;
            taskDto.Type = type;
            taskDto.IsProbable = isProbable;
            if (withDate)
                taskDto.Date = CreateDateTime(year, month, day, dayAdjustment);
            if (withTime)
                taskDto.Time = hour * 60 + minutes;
                
            return taskDto;
        }

        private string ParseFlags(string task, out bool isProbable, out TaskTypeEnum type)
        {
            var flagsRx = new Regex(@"\s[\?!]+$");
            
            isProbable = false;
            type = TaskTypeEnum.Simple;

            if (!flagsRx.IsMatch(task))
                return task;

            foreach (var f in task.Split(' ').Last())
            {
                if (f == '?')
                    isProbable = true;
                else if (f == '!')
                    type = TaskTypeEnum.Additional;
            }

            return flagsRx.Replace(task, "");
        }

        private string ParseTime(string task, out int hour, out int minutes, out bool withTime)
        {
            var timeRx = new Regex(@"^\d{4}\s");
            string time = string.Empty;
            hour = 0;
            minutes = 0;
            withTime = false;

            if (timeRx.IsMatch(task))
            {
                time = task.Substring(0, 4);
                task = task.Substring(5);
                withTime = true;
            }

            if (!string.IsNullOrEmpty(time))
            {
                hour = int.Parse(time.Substring(0, 2));
                minutes = int.Parse(time.Substring(2, 2));
            }

            return task;
        }

        private string ParseDate(string task, out int year, out int month, out int day, out bool withDate, out int dayAdjustment)
        {
            var dateWithYearRx = new Regex(@"^\d{8}\s");
            var dateRx = new Regex(@"^\d{4}\s");
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
                task = task.Substring(9);
            }
            else if (dateRx.IsMatch(task))
            {
                date = task.Substring(0, 4);
                year = _dateService.Today.Year;
                task = task.Substring(5);
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

        private DateTime CreateDateTime(int year, int month, int day, int dayAdjustment)
        {
            var dateTime = new DateTime(year, month, day, 0, 0, 0);
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            dateTime = dateTime.AddDays(dayAdjustment);
            return dateTime;
        }

        public string PrintTasks(IEnumerable<TaskDto> tasks)
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

        private string TaskToString(TaskDto task)
        {
            string result = string.Empty;

            if (task.Time.HasValue)
                result += $"{TimeToString(task.Time.Value)} ";

            result += task.Title;

            return result;
        }

        private string TimeToString(int time)
        {
            int hour = time / 60;
            int minute = time % 60;
            return $"{hour:D2}:{minute:D2}";  
        } 
    }
}