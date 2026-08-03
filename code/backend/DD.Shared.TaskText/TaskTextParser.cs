using System.Globalization;
using System.Text.RegularExpressions;

namespace DD.Shared.TaskText;

public interface ITaskTextParser
{
    IReadOnlyList<ParsedTaskText> Parse(string text);

    ParsedTaskText ParseTemplate(string text);
}

public class TaskTextParser(ITaskTextDateProvider dateProvider) : ITaskTextParser
{
    public const int MinRangeDays = 2;
    public const int MaxRangeDays = 31;

    public IReadOnlyList<ParsedTaskText> Parse(string text)
    {
        if (TryParseDateRange(text, out var startDate, out var endDate, out var remaining))
        {
            ValidateRange(startDate, endDate);

            var template = ParseTemplate(remaining);
            var tasks = new List<ParsedTaskText>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
                tasks.Add(template with { Date = date });

            return tasks;
        }

        return [ParseSingle(text)];
    }

    // Parses a dateless task template: time + flags + title only, without any date.
    public ParsedTaskText ParseTemplate(string text)
    {
        text = ParseTime(text, out var hour, out var minutes, out var withTime);
        text = ParseFlags(text, out var isProbable, out var type);

        return new ParsedTaskText(
            Date: null,
            Time: withTime ? hour * 60 + minutes : null,
            Title: text,
            Type: type,
            IsProbable: isProbable);
    }

    private static void ValidateRange(DateOnly startDate, DateOnly endDate)
    {
        // A range must span at least two days (i.e. produce at least two tasks) and at most
        // MaxRangeDays. A reversed range yields a negative count, which is also below MinRangeDays.
        var dayCount = endDate.DayNumber - startDate.DayNumber + 1;
        if (dayCount is < MinRangeDays or > MaxRangeDays)
            throw new TaskTextParseException("Date range is invalid or exceeds the allowed number of days.");
    }

    private static string ParseFlags(string task, out bool isProbable, out TaskTextType type)
    {
        var flagsRx = new Regex(@"\s[?!*%]+$");

        isProbable = false;
        type = TaskTextType.Simple;

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
                        type = TaskTextType.Simple;
                        isProbable = false;
                        return task;
                    }

                    isProbable = true;
                    break;

                case '!':
                    if (type != TaskTextType.Simple)
                    {
                        type = TaskTextType.Simple;
                        isProbable = false;
                        return task;
                    }

                    type = TaskTextType.Additional;
                    break;

                case '*':
                    if (type != TaskTextType.Simple)
                    {
                        type = TaskTextType.Simple;
                        isProbable = false;
                        return task;
                    }

                    type = TaskTextType.Routine;
                    break;

                case '%':
                    if (type != TaskTextType.Simple)
                    {
                        type = TaskTextType.Simple;
                        isProbable = false;
                        return task;
                    }

                    type = TaskTextType.Weekly;
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

    private static string ParseTodayShift(string task, out int dayAdjustment)
    {
        dayAdjustment = new Regex("!+").Matches(task)[0].Length;
        dayAdjustment--;
        return task[(dayAdjustment + 2)..];
    }

    private static DateOnly CreateDate(int year, int month, int day, int dayAdjustment)
    {
        try
        {
            return new DateOnly(year, month, day).AddDays(dayAdjustment);
        }
        catch (ArgumentOutOfRangeException e)
        {
            throw new TaskTextParseException("Task text contains an invalid calendar date.", e);
        }
    }

    private bool TryParseDateRange(string task, out DateOnly startDate, out DateOnly endDate, out string remaining)
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

        startDate = CreateDate(startYear, startMonth, startDay, 0);
        endDate = CreateDate(endYear, endMonth, endDay, 0);
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
            year = dateProvider.Today.Year;
            month = int.Parse(token[..2], CultureInfo.InvariantCulture);
            day = int.Parse(token[2..4], CultureInfo.InvariantCulture);
        }
    }

    private ParsedTaskText ParseSingle(string task)
    {
        task = ParseDate(task, out var year, out var month, out var day, out var withDate, out var dayAdjustment);

        var template = ParseTemplate(task);
        return withDate
            ? template with { Date = CreateDate(year, month, day, dayAdjustment) }
            : template;
    }

    private string ParseDate(string task, out int year, out int month, out int day, out bool withDate, out int dayAdjustment)
    {
        var dateWithYearRx = new Regex(@"^\d{8}\s");
        var dateRx = new Regex(@"^\d{4}\s");
        var todayShiftRx = new Regex(@"^!+\s");
        var weekShiftRx = new Regex(@"^!+[1-7]\s");
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
            year = dateProvider.Today.Year;
            month = dateProvider.Today.Month;
            day = dateProvider.Today.Day;
            withDate = true;
        }
        else if (weekShiftRx.IsMatch(task))
        {
            task = ParseWeekShift(task, out dayAdjustment);
            year = dateProvider.Today.Year;
            month = dateProvider.Today.Month;
            day = dateProvider.Today.Day;
            withDate = true;
        }

        return task;
    }

    private string ParseWeekShift(string task, out int dayAdjustment)
    {
        var weekCount = new Regex("^!+").Match(task).Length;
        var weekday = int.Parse(task[weekCount].ToString(), CultureInfo.InvariantCulture);
        var todayWeekday = (int)dateProvider.Today.DayOfWeek;
        var thisWeekMondayShift = todayWeekday == 0 ? -6 : 1 - todayWeekday;
        dayAdjustment = thisWeekMondayShift + (weekday - 1) + 7 * (weekCount - 1);
        return task[(weekCount + 2)..];
    }
}
