using DD.Shared.TaskText;
using DD.TerminalClient.Domain.Editing;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Time;
using Moq;
using Xunit;

namespace DD.Tests.Unit.TerminalClient;

// Verifies TaskTextFormatter produces the web-compatible editor string and that the shared
// TaskTextParser round-trips it back into the same fields (formatter and parser share the same
// injected "today" year, matching how the terminal client wires them).
public class TaskTextFormatterTests
{
    private const int TodayYear = 2019;

    [Fact]
    public void Format_DatedCurrentYearNoTime_ElidesYear()
    {
        var task = NewTask(date: new DateOnly(2019, 3, 5), title: "Title");

        Assert.Equal("0305 Title", Formatter().Format(task));
    }

    [Fact]
    public void Format_DatedWithTime_EmitsTime()
    {
        var task = NewTask(date: new DateOnly(2019, 3, 5), time: 1050, title: "Title");

        Assert.Equal("0305 1730 Title", Formatter().Format(task));
    }

    [Fact]
    public void Format_DatedOtherYear_IncludesYear()
    {
        var task = NewTask(date: new DateOnly(2020, 3, 5), title: "Title");

        Assert.Equal("20200305 Title", Formatter().Format(task));
    }

    [Fact]
    public void Format_NoDateSimple_IsTitleOnly()
    {
        var task = NewTask(title: "Title");

        Assert.Equal("Title", Formatter().Format(task));
    }

    [Theory]
    [InlineData(TerminalTaskType.Additional, false, "Title !")]
    [InlineData(TerminalTaskType.Routine, false, "Title *")]
    [InlineData(TerminalTaskType.Weekly, false, "Title %")]
    [InlineData(TerminalTaskType.Simple, true, "Title ?")]
    [InlineData(TerminalTaskType.Additional, true, "Title !?")]
    public void Format_Flags_AppendsSuffix(TerminalTaskType type, bool isProbable, string expected)
    {
        var task = NewTask(title: "Title", type: type, isProbable: isProbable);

        Assert.Equal(expected, Formatter().Format(task));
    }

    [Fact]
    public void Format_DatedTimedWithFlags_CombinesAll()
    {
        var task = NewTask(
            date: new DateOnly(2019, 3, 5),
            time: 1050,
            title: "Title",
            type: TerminalTaskType.Routine,
            isProbable: true);

        Assert.Equal("0305 1730 Title *?", Formatter().Format(task));
    }

    [Fact]
    public void Format_ParsedTaskTextOverload_MatchesModel()
    {
        var parsed = new ParsedTaskText(new DateOnly(2019, 3, 5), 1050, "Title", TaskTextType.Additional, false);

        Assert.Equal("0305 1730 Title !", Formatter().Format(parsed));
    }

    public static IEnumerable<object[]> RoundTripCases()
    {
        yield return [NewTask(date: new DateOnly(2019, 3, 5), time: 1050, title: "Buy milk")];
        yield return [NewTask(date: new DateOnly(2020, 12, 31), time: 1439, title: "New Year Eve", type: TerminalTaskType.Additional)];
        yield return [NewTask(title: "No date task", type: TerminalTaskType.Routine, isProbable: true)];
        yield return [NewTask(date: new DateOnly(2019, 1, 1), title: "Weekly probable", type: TerminalTaskType.Weekly, isProbable: true)];
    }

    [Theory]
    [MemberData(nameof(RoundTripCases))]
    public void Format_ThenParse_RoundTripsFields(TerminalTask task)
    {
        var text = Formatter().Format(task);

        var parsed = Parser().Parse(text).Single();

        Assert.Equal(task.Date, parsed.Date);
        Assert.Equal(task.Time, parsed.Time);
        Assert.Equal(task.Title, parsed.Title);
        Assert.Equal((int)task.Type, (int)parsed.Type);
        Assert.Equal(task.IsProbable, parsed.IsProbable);
    }

    private static TaskTextFormatter Formatter(int year = TodayYear)
    {
        var provider = new Mock<ILocalDateProvider>();
        provider.SetupGet(x => x.Today).Returns(new DateOnly(year, 1, 1));
        return new TaskTextFormatter(provider.Object);
    }

    private static TaskTextParser Parser(int year = TodayYear)
    {
        var provider = new Mock<ITaskTextDateProvider>();
        provider.SetupGet(x => x.Today).Returns(new DateOnly(year, 1, 1));
        return new TaskTextParser(provider.Object);
    }

    private static TerminalTask NewTask(
        DateOnly? date = null,
        int? time = null,
        string title = "",
        TerminalTaskType type = TerminalTaskType.Simple,
        bool isProbable = false)
    {
        return new TerminalTask
        {
            Uid = "uid",
            Title = title,
            Date = date,
            Time = time,
            Order = 0,
            Completed = false,
            Deleted = false,
            Type = type,
            IsProbable = isProbable,
            Version = 0,
        };
    }
}
