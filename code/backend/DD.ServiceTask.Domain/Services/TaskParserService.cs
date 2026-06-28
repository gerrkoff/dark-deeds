using System.Globalization;
using System.Text.RegularExpressions;
using DD.ServiceTask.Domain.Exceptions;
using DD.Shared.Details.Abstractions.Dto;

namespace DD.ServiceTask.Domain.Services;

public interface ITaskParserService
{
    IReadOnlyList<TaskDto> ParseTasks(string task);

    TaskDto ParseTaskTemplate(string task);
}

public class TaskParserService(IDateService dateService) : ITaskParserService
{
    private const int MinRangeDays = 2;
    private const int MaxRangeDays = 31;

    public IReadOnlyList<TaskDto> ParseTasks(string task)
    {
        if (TryParseDateRange(task, out var startDate, out var endDate, out var remaining))
        {
            ValidateRange(startDate, endDate);

            var template = ParseTaskTemplate(remaining);
            var tasks = new List<TaskDto>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
                tasks.Add(CloneWithDate(template, date));

            return tasks;
        }

        return [ParseSingle(task)];
    }

    // Parses a dateless task template: time + flags + title only, without any date.
    public TaskDto ParseTaskTemplate(string task)
    {
        task = ParseTime(task, out var hour, out var minutes, out var withTime);
        task = ParseFlags(task, out var isProbable, out var type);

        var taskDto = new TaskDto
        {
            Title = task,
            Type = type,
            IsProbable = isProbable,
        };
        if (withTime)
            taskDto.Time = hour * 60 + minutes;

        return taskDto;
    }

    private TaskDto ParseSingle(string task)
    {
        task = ParseDate(task, out var year, out var month, out var day, out var withDate, out var dayAdjustment);

        var taskDto = ParseTaskTemplate(task);
        if (withDate)
            taskDto.Date = CreateDateTime(year, month, day, dayAdjustment);

        return taskDto;
    }

    private bool TryParseDateRange(string task, out DateTime startDate, out DateTime endDate, out string remaining)
    {
        var rangeRx = new Regex(@"^(\d{8}|\d{4})-(\d{8}|\d{4})\s");
        var match = rangeRx.Match(task);

        startDate = default;
        endDate = default;
        remaining = task;

        if (!match.Success)
            return false;

        ParseStringDate(match.Groups[1].Value, out var startYear, out var startMonth, out var startDay);
        ParseStringDate(match.Groups[2].Value, out var endYear, out var endMonth, out var endDay);

        startDate = CreateDateTime(startYear, startMonth, startDay, 0);
        endDate = CreateDateTime(endYear, endMonth, endDay, 0);
        remaining = task[match.Length..];
        return true;
    }

    // Parses a numeric date token in either MMDD (current year) or YYYYMMDD form.
    // Shared by the single-date path (ParseDate) and the date-range path (TryParseDateRange).
    private void ParseStringDate(string token, out int year, out int month, out int day)
    {
        if (token.Length == 8)
        {
            year = int.Parse(token[..4], CultureInfo.InvariantCulture);
            month = int.Parse(token[4..6], CultureInfo.InvariantCulture);
            day = int.Parse(token[6..8], CultureInfo.InvariantCulture);
        }
        else
        {
            year = dateService.Today.Year;
            month = int.Parse(token[..2], CultureInfo.InvariantCulture);
            day = int.Parse(token[2..4], CultureInfo.InvariantCulture);
        }
    }

    private static void ValidateRange(DateTime startDate, DateTime endDate)
    {
        // A range must span at least two days (i.e. produce at least two tasks) and at most
        // MaxRangeDays. A reversed range yields a negative count, which is also below MinRangeDays.
        var dayCount = (endDate - startDate).Days + 1;
        if (dayCount is < MinRangeDays or > MaxRangeDays)
            throw ServiceException.InvalidDateRange(MaxRangeDays);
    }

    private static TaskDto CloneWithDate(TaskDto template, DateTime date)
    {
        return new TaskDto
        {
            Id = template.Id,
            Date = date,
            Time = template.Time,
            Title = template.Title,
            Order = template.Order,
            Completed = template.Completed,
            IsProbable = template.IsProbable,
            Deleted = template.Deleted,
            Type = template.Type,
            Version = template.Version,
            Uid = template.Uid,
        };
    }

    private static string ParseFlags(string task, out bool isProbable, out TaskTypeDto type)
    {
        var flagsRx = new Regex(@"\s[?!*%]+$");

        isProbable = false;
        type = TaskTypeDto.Simple;

        if (!flagsRx.IsMatch(task))
            return task;

        var token = task.Split(' ').Last();
        foreach (var f in token)
        {
            switch (f)
            {
                case '?':
                    if (isProbable)
                    {
                        type = TaskTypeDto.Simple;
                        isProbable = false;
                        return task;
                    }

                    isProbable = true;
                    break;

                case '!':
                    if (type != TaskTypeDto.Simple)
                    {
                        type = TaskTypeDto.Simple;
                        isProbable = false;
                        return task;
                    }

                    type = TaskTypeDto.Additional;
                    break;

                case '*':
                    if (type != TaskTypeDto.Simple)
                    {
                        type = TaskTypeDto.Simple;
                        isProbable = false;
                        return task;
                    }

                    type = TaskTypeDto.Routine;
                    break;

                case '%':
                    if (type != TaskTypeDto.Simple)
                    {
                        type = TaskTypeDto.Simple;
                        isProbable = false;
                        return task;
                    }

                    type = TaskTypeDto.Weekly;
                    break;
            }
        }

        return flagsRx.Replace(task, string.Empty);
    }

    private static string ParseTime(string task, out int hour, out int minutes, out bool withTime)
    {
        var timeRx = new Regex(@"^\d{4}\s");
        var time = string.Empty;
        hour = 0;
        minutes = 0;
        withTime = false;

        if (timeRx.IsMatch(task))
        {
            time = task[..4];
            task = task[5..];
            withTime = true;
        }

        if (!string.IsNullOrEmpty(time))
        {
            hour = int.Parse(time[..2], CultureInfo.InvariantCulture);
            minutes = int.Parse(time[2..4], CultureInfo.InvariantCulture);
        }

        return task;
    }

    private string ParseDate(string task, out int year, out int month, out int day, out bool withDate, out int dayAdjustment)
    {
        var dateWithYearRx = new Regex(@"^\d{8}\s");
        var dateRx = new Regex(@"^\d{4}\s");
        var todayShiftRx = new Regex(@"^!+\s");
        var weekShiftRx = new Regex(@"^![1-7]\s");
        year = 0;
        month = 0;
        day = 0;
        dayAdjustment = 0;
        withDate = false;

        if (dateWithYearRx.IsMatch(task))
        {
            ParseStringDate(task[..8], out year, out month, out day);
            task = task[9..];
            withDate = true;
        }
        else if (dateRx.IsMatch(task))
        {
            ParseStringDate(task[..4], out year, out month, out day);
            task = task[5..];
            withDate = true;
        }
        else if (todayShiftRx.IsMatch(task))
        {
            task = ParseTodayShift(task, out dayAdjustment);
            year = dateService.Today.Year;
            month = dateService.Today.Month;
            day = dateService.Today.Day;
            withDate = true;
        }
        else if (weekShiftRx.IsMatch(task))
        {
            task = ParseWeekShift(task, out dayAdjustment);
            year = dateService.Today.Year;
            month = dateService.Today.Month;
            day = dateService.Today.Day;
            withDate = true;
        }

        return task;
    }

    private string ParseWeekShift(string task, out int dayAdjustment)
    {
        var dayShift = int.Parse(task[1].ToString(), CultureInfo.InvariantCulture);
        var nextSundayShift = (7 - (int)dateService.Today.DayOfWeek) % 7;
        dayAdjustment = nextSundayShift + dayShift;
        return task[3..];
    }

    private static string ParseTodayShift(string task, out int dayAdjustment)
    {
        dayAdjustment = new Regex("!+").Matches(task)[0].Length;
        dayAdjustment--;
        return task[(dayAdjustment + 2)..];
    }

    private static DateTime CreateDateTime(int year, int month, int day, int dayAdjustment)
    {
        var dateTime = new DateTime(year, month, day, 0, 0, 0);
        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        dateTime = dateTime.AddDays(dayAdjustment);
        return dateTime;
    }
}
