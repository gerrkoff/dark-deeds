using DD.Shared.TaskText;
using Moq;
using Xunit;

namespace DD.Tests.Unit.Shared;

// These mirror BE TaskParserServiceTest (#1-#39) and the FE TaskConvertService / TaskRangeService
// numbered cases, but assert the pure shared parser directly (DateOnly, TaskTextType,
// TaskTextParseException) with no TaskDto / DI dependency.
public class TaskTextParserTests
{
    // #1
    [Fact]
    public void Parse_NoDateAndNoTime()
    {
        var result = Parser().Parse("Test!").Single();

        Assert.Equal("Test!", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #2
    [Fact]
    public void Parse_DateAndNoTime()
    {
        var result = Parser().Parse("1231 Test!").Single();

        Assert.Equal("Test!", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.Equal(new DateOnly(2019, 12, 31), result.Date);
        Assert.Null(result.Time);
    }

    // #3
    [Fact]
    public void Parse_DateAndNoTime_NotWorkingWithoutSpace()
    {
        var result = Parser().Parse("0101Test!!!").Single();

        Assert.Equal("0101Test!!!", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #4
    [Fact]
    public void Parse_DateAndTime()
    {
        var result = Parser().Parse("1231 2359 Test!").Single();

        Assert.Equal("Test!", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.Equal(new DateOnly(2019, 12, 31), result.Date);
        Assert.Equal(1439, result.Time);
    }

    // #5
    [Fact]
    public void Parse_DateAndTime_NotWorkingWithoutSpace()
    {
        var result = Parser().Parse("0101 0101Test!!!").Single();

        Assert.Equal("0101Test!!!", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.Equal(new DateOnly(2019, 1, 1), result.Date);
        Assert.Null(result.Time);
    }

    // #6
    [Fact]
    public void Parse_DateAndNoTimeWithYear()
    {
        var result = Parser().Parse("20170101 Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.Equal(new DateOnly(2017, 1, 1), result.Date);
        Assert.Null(result.Time);
    }

    // #7
    [Fact]
    public void Parse_ProbableTask()
    {
        var result = Parser().Parse("Test! ?").Single();

        Assert.Equal("Test!", result.Title);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #8
    [Fact]
    public void Parse_AdditionalTaskWithDate()
    {
        var result = Parser().Parse("0220 Test !").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Additional, result.Type);
        Assert.Equal(new DateOnly(2019, 2, 20), result.Date);
        Assert.Null(result.Time);
    }

    // #9
    [Fact]
    public void Parse_AdditionalTaskWithDateAndTime()
    {
        var result = Parser().Parse("20150220 2359 Test !").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Additional, result.Type);
        Assert.Equal(new DateOnly(2015, 2, 20), result.Date);
        Assert.Equal(1439, result.Time);
    }

    // #10
    [Fact]
    public void Parse_AdditionalAndProbable()
    {
        var result = Parser().Parse("Test !?").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Additional, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #10.1
    [Fact]
    public void Parse_ProbableAndAdditional()
    {
        var result = Parser().Parse("Test ?!").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Additional, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #11
    [Fact]
    public void Parse_TodayTaskThroughExclamationMark()
    {
        var result = Parser().Parse("! Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.Equal(new DateOnly(2019, 1, 1), result.Date);
        Assert.Null(result.Time);
    }

    // #12
    [Fact]
    public void Parse_TomorrowTaskThroughExclamationMark()
    {
        var result = Parser().Parse("!! Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.Equal(new DateOnly(2019, 1, 2), result.Date);
        Assert.Null(result.Time);
    }

    // #13
    [Fact]
    public void Parse_DayAfterAfterTomorrowTaskThroughExclamationMark()
    {
        var result = Parser().Parse("!!!! Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.Equal(new DateOnly(2019, 1, 4), result.Date);
        Assert.Null(result.Time);
    }

    // #14
    [Fact]
    public void Parse_DayAfterTomorrowNextMonthTaskThroughExclamationMark()
    {
        var result = Parser(2019, 1, 31).Parse("!!! Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.Equal(new DateOnly(2019, 2, 2), result.Date);
        Assert.Null(result.Time);
    }

    // #15
    [Fact]
    public void Parse_ThisWeekMondayTaskThroughExclamationMark()
    {
        var result = Parser(2019, 7, 31).Parse("!1 Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.Equal(new DateOnly(2019, 7, 29), result.Date);
        Assert.Null(result.Time);
    }

    // #15.1
    [Fact]
    public void Parse_NextMondayTaskThroughExclamationMark()
    {
        var result = Parser(2019, 7, 28).Parse("!!1 Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.Equal(new DateOnly(2019, 7, 29), result.Date);
        Assert.Null(result.Time);
    }

    // #16
    [Fact]
    public void Parse_NextWednesdayTaskThroughExclamationMark()
    {
        var result = Parser(2019, 7, 28).Parse("!!3 Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.Equal(new DateOnly(2019, 7, 31), result.Date);
        Assert.Null(result.Time);
    }

    // #17
    [Fact]
    public void Parse_NextFridayTaskThroughExclamationMark()
    {
        var result = Parser(2019, 7, 28).Parse("!!5 Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.Equal(new DateOnly(2019, 8, 2), result.Date);
        Assert.Null(result.Time);
    }

    // #17.1
    [Fact]
    public void Parse_WeekAfterNextMondayTaskThroughExclamationMark()
    {
        var result = Parser(2019, 7, 28).Parse("!!!1 Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.Equal(new DateOnly(2019, 8, 5), result.Date);
        Assert.Null(result.Time);
    }

    // #18
    [Fact]
    public void Parse_ExclamationMark11IsNotWeekShiftPattern()
    {
        var result = Parser().Parse("!11 Test").Single();

        Assert.Equal("!11 Test", result.Title);
        Assert.Null(result.Date);
    }

    // #19
    [Fact]
    public void Parse_DateWithExclamation()
    {
        var result = Parser().Parse("1231! Test").Single();

        Assert.Equal("1231! Test", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #20
    [Fact]
    public void Parse_RoutineTaskWithDate()
    {
        var result = Parser().Parse("0220 Test *").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Routine, result.Type);
        Assert.Equal(new DateOnly(2019, 2, 20), result.Date);
        Assert.Null(result.Time);
    }

    // #21
    [Fact]
    public void Parse_RoutineTaskWithDateAndTime()
    {
        var result = Parser().Parse("20150220 2359 Test *").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Routine, result.Type);
        Assert.Equal(new DateOnly(2015, 2, 20), result.Date);
        Assert.Equal(1439, result.Time);
    }

    // #22
    [Fact]
    public void Parse_RoutineAndProbable()
    {
        var result = Parser().Parse("Test *?").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Routine, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #22.1
    [Fact]
    public void Parse_ProbableAndRoutine()
    {
        var result = Parser().Parse("Test ?*").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Routine, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #23
    [Fact]
    public void Parse_RoutineAndAdditionalConflict()
    {
        var result = Parser().Parse("Test !*").Single();

        Assert.Equal("Test !*", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.False(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #24
    [Fact]
    public void Parse_WeeklyTask()
    {
        var result = Parser().Parse("Test %").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Weekly, result.Type);
        Assert.False(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #25
    [Fact]
    public void Parse_WeeklyAndProbable()
    {
        var result = Parser().Parse("Test %?").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Weekly, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #25.1
    [Fact]
    public void Parse_ProbableAndWeekly()
    {
        var result = Parser().Parse("Test ?%").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTextType.Weekly, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #26
    [Fact]
    public void Parse_WeeklyDuplicateCancels()
    {
        var result = Parser().Parse("Test %%").Single();

        Assert.Equal("Test %%", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.False(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #27
    [Fact]
    public void Parse_WeeklyAndRoutineConflict()
    {
        var result = Parser().Parse("Test %*").Single();

        Assert.Equal("Test %*", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.False(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #28
    [Fact]
    public void Parse_ProbableWeeklyProbableConflict()
    {
        var result = Parser().Parse("Test ?%?").Single();

        Assert.Equal("Test ?%?", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.False(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    [Fact]
    public void ParseTemplate_TaskWithTime_SkipsDate()
    {
        var result = Parser().ParseTemplate("1010 Test!");

        Assert.Equal("Test!", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.Null(result.Date);
        Assert.Equal(610, result.Time);
    }

    // #29
    [Fact]
    public void Parse_DateRangeWithoutYear_CreatesTaskPerInclusiveDay()
    {
        var result = Parser().Parse("0909-0913 venice");

        Assert.Equal(5, result.Count);
        Assert.Equal(new DateOnly(2019, 9, 9), result[0].Date);
        Assert.Equal(new DateOnly(2019, 9, 13), result[4].Date);
        Assert.All(result, x => Assert.Equal("venice", x.Title));
    }

    // #30
    [Fact]
    public void Parse_DateRangeWithTimeAndType_AppliesToEveryTask()
    {
        var result = Parser().Parse("0909-0913 1012 venice !");

        Assert.Equal(5, result.Count);
        Assert.Equal(new DateOnly(2019, 9, 9), result[0].Date);
        Assert.Equal(new DateOnly(2019, 9, 13), result[4].Date);
        Assert.All(result, x =>
        {
            Assert.Equal("venice", x.Title);
            Assert.Equal(TaskTextType.Additional, x.Type);
            Assert.Equal(612, x.Time);
        });
    }

    // #31
    [Fact]
    public void Parse_DateRangeWithYearOnBothEndpoints_ParsesYears()
    {
        var result = Parser().Parse("20270909-20270915 venice");

        Assert.Equal(7, result.Count);
        Assert.Equal(new DateOnly(2027, 9, 9), result[0].Date);
        Assert.Equal(new DateOnly(2027, 9, 15), result[6].Date);
    }

    // #32
    [Fact]
    public void Parse_DateRangeWithMixedYearEndpoints_ParsesEachEndpointIndependently()
    {
        var result = Parser().Parse("0909-20190915 venice");

        Assert.Equal(7, result.Count);
        Assert.Equal(new DateOnly(2019, 9, 9), result[0].Date);
        Assert.Equal(new DateOnly(2019, 9, 15), result[6].Date);
    }

    // #33
    [Fact]
    public void Parse_DateRangeWithoutSpace_NotTreatedAsRange()
    {
        var result = Parser().Parse("0909-0913venice");

        Assert.Single(result);
        Assert.Equal("0909-0913venice", result[0].Title);
        Assert.Null(result[0].Date);
    }

    // #34
    [Fact]
    public void Parse_SingleDayRange_Throws()
    {
        Assert.Throws<TaskTextParseException>(() => Parser().Parse("0909-0909 venice"));
    }

    // #35
    [Fact]
    public void Parse_DateRangeAtLimit_CreatesTasks()
    {
        var result = Parser().Parse("0901-1001 venice");

        Assert.Equal(31, result.Count);
        Assert.Equal(new DateOnly(2019, 9, 1), result[0].Date);
        Assert.Equal(new DateOnly(2019, 10, 1), result[30].Date);
    }

    // #36
    [Fact]
    public void Parse_DateRangeExceedingLimit_Throws()
    {
        Assert.Throws<TaskTextParseException>(() => Parser().Parse("0901-1002 venice"));
    }

    // #37
    [Fact]
    public void Parse_MultiYearDateRange_Throws()
    {
        Assert.Throws<TaskTextParseException>(() => Parser().Parse("20270909-20280913 venice"));
    }

    // #38
    [Fact]
    public void Parse_ReversedDateRange_Throws()
    {
        Assert.Throws<TaskTextParseException>(() => Parser().Parse("0913-0909 venice"));
    }

    // #39
    [Fact]
    public void Parse_MinimumValidRange_CreatesTwoTasks()
    {
        var result = Parser().Parse("0910-0911 venice");

        Assert.Equal(2, result.Count);
        Assert.Equal(new DateOnly(2019, 9, 10), result[0].Date);
        Assert.Equal(new DateOnly(2019, 9, 11), result[1].Date);
    }

    // [Explicit edge cases required by the plan]
    [Fact]
    public void Parse_InvalidCalendarDate_Throws()
    {
        Assert.Throws<TaskTextParseException>(() => Parser().Parse("1332 Test"));
    }

    [Fact]
    public void Parse_InvalidCalendarDateWithYear_Throws()
    {
        Assert.Throws<TaskTextParseException>(() => Parser().Parse("20190230 Test"));
    }

    [Fact]
    public void Parse_InvalidCalendarDateRangeEndpoint_Throws()
    {
        Assert.Throws<TaskTextParseException>(() => Parser().Parse("0230-0301 venice"));
    }

    [Fact]
    public void Parse_DuplicateAdditionalFlagCancels()
    {
        var result = Parser().Parse("Test !!").Single();

        Assert.Equal("Test !!", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.False(result.IsProbable);
    }

    [Fact]
    public void Parse_DuplicateRoutineFlagCancels()
    {
        var result = Parser().Parse("Test **").Single();

        Assert.Equal("Test **", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.False(result.IsProbable);
    }

    [Fact]
    public void Parse_DuplicateProbableFlagCancels()
    {
        var result = Parser().Parse("Test ??").Single();

        Assert.Equal("Test ??", result.Title);
        Assert.Equal(TaskTextType.Simple, result.Type);
        Assert.False(result.IsProbable);
    }

    [Fact]
    public void Parse_InjectedCurrentDate_UsedForRelativeYearlessDate()
    {
        var result = Parser(2022, 5, 10).Parse("0715 Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(new DateOnly(2022, 7, 15), result.Date);
    }

    [Fact]
    public void Parse_InjectedCurrentDate_UsedForTodayShift()
    {
        var result = Parser(2020, 6, 15).Parse("! Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(new DateOnly(2020, 6, 15), result.Date);
    }

    private static TaskTextParser Parser(int year = 2019, int month = 1, int day = 1)
    {
        var provider = new Mock<ITaskTextDateProvider>();
        provider.SetupGet(x => x.Today).Returns(new DateOnly(year, month, day));
        return new TaskTextParser(provider.Object);
    }
}
