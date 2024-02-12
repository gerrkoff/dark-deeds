using System.Globalization;
using System.Text.RegularExpressions;
using DD.ServiceTask.Domain.Dto;
using DD.ServiceTask.Domain.Entities.Enums;

namespace DD.ServiceTask.Domain.Services;

public interface ITaskParserService
{
    TaskDto ParseTask(string task, bool ignoreDate = false);

    // TODO: extract to telegram client
    IList<string> PrintTasks(IEnumerable<TaskDto> tasks);
}

public class TaskParserService(IDateService dateService) : ITaskParserService
{
    public TaskDto ParseTask(string task, bool ignoreDate = false)
    {
        var taskDto = new TaskDto();
        int year = 0, month = 0, day = 0, dayAdjustment = 0;
        var withDate = false;

        if (!ignoreDate)
            task = ParseDate(task, out year, out month, out day, out withDate, out dayAdjustment);
        task = ParseTime(task, out var hour, out var minutes, out var withTime);
        task = ParseFlags(task, out var isProbable, out var type);

        taskDto.Title = task;
        taskDto.Type = type;
        taskDto.IsProbable = isProbable;
        if (withDate)
            taskDto.Date = CreateDateTime(year, month, day, dayAdjustment);
        if (withTime)
            taskDto.Time = hour * 60 + minutes;

        return taskDto;
    }

    public IList<string> PrintTasks(IEnumerable<TaskDto> tasks)
    {
        return tasks.Select(TaskToString).ToList();
    }

    private static string ParseFlags(string task, out bool isProbable, out TaskType type)
    {
        var flagsRx = new Regex(@"\s[\?!]+$");

        isProbable = false;
        type = TaskType.Simple;

        if (!flagsRx.IsMatch(task))
            return task;

        foreach (var f in task.Split(' ').Last())
        {
            if (f == '?')
                isProbable = true;
            else if (f == '!')
                type = TaskType.Additional;
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
        var date = string.Empty;
        year = 0;
        month = 0;
        day = 0;
        dayAdjustment = 0;
        withDate = false;

        if (dateWithYearRx.IsMatch(task))
        {
            date = task[4..8];
            year = int.Parse(task[..4], CultureInfo.InvariantCulture);
            task = task[9..];
        }
        else if (dateRx.IsMatch(task))
        {
            date = task[..4];
            year = dateService.Today.Year;
            task = task[5..];
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

        if (!string.IsNullOrEmpty(date))
        {
            month = int.Parse(date[..2], CultureInfo.InvariantCulture);
            day = int.Parse(date[2..4], CultureInfo.InvariantCulture);
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

    private string TaskToString(TaskDto task)
    {
        var result = string.Empty;

        if (task.Time.HasValue)
            result += $"{TimeToString(task.Time.Value)} ";

        result += task.Title;

        return result;
    }

    private static string TimeToString(int time)
    {
        var hour = time / 60;
        var minute = time % 60;
        return $"{hour:D2}:{minute:D2}";
    }
}
