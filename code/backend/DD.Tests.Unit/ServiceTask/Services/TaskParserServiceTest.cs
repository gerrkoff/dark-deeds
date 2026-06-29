using DD.ServiceTask.Domain.Exceptions;
using DD.ServiceTask.Domain.Services;
using DD.Shared.Details.Abstractions.Dto;
using Moq;
using Xunit;

namespace DD.Tests.Unit.ServiceTask.Services;

public class TaskParserServiceTest : BaseTest
{
    // These tests should be synced with FE TaskConverter.convertStringToModel tests

    // #1
    [Fact]
    public void ParseTasks_ReturnTaskWithNoDateAndNoTime()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("Test!").Single();

        Assert.Equal("Test!", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #2
    [Fact]
    public void ParseTasks_ReturnTaskWithDateAndNoTime()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("1231 Test!").Single();

        Assert.Equal("Test!", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 12, 31, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #3
    [Fact]
    public void ParseTasks_ReturnTaskWithDateAndNoTime_NotWorkingWithoutSpace()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("0101Test!!!").Single();

        Assert.Equal("0101Test!!!", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #4
    [Fact]
    public void ParseTasks_ReturnTaskWithDateAndTime()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("1231 2359 Test!").Single();

        Assert.Equal("Test!", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 12, 31, 0, 0, 0), result.Date);
        Assert.Equal(1439, result.Time);
    }

    // #5
    [Fact]
    public void ParseTasks_ReturnTaskWithDateAndTime_NotWorkingWithoutSpace()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("0101 0101Test!!!").Single();

        Assert.Equal("0101Test!!!", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 1, 1, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #6
    [Fact]
    public void ParseTasks_ReturnTaskWithDateAndNoTimeWithYear()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("20170101 Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2017, 1, 1, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #7
    [Fact]
    public void ParseTasks_ReturnProbableTask()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("Test! ?").Single();

        Assert.Equal("Test!", result.Title);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #8
    [Fact]
    public void ParseTasks_ReturnAdditionalTaskWithDate()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("0220 Test !").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Additional, result.Type);
        Assert.Equal(new DateTime(2019, 2, 20, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #9
    [Fact]
    public void ParseTasks_ReturnAdditionalTaskWithDateAndTime()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("20150220 2359 Test !").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Additional, result.Type);
        Assert.Equal(new DateTime(2015, 2, 20, 0, 0, 0), result.Date);
        Assert.Equal(1439, result.Time);
    }

    // #10
    [Fact]
    public void ParseTasks_AdditionalAndProbable()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("Test !?").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Additional, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #10.1
    [Fact]
    public void ParseTasks_ProbableAndAdditional()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("Test ?!").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Additional, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #11
    [Fact]
    public void ParseTasks_ReturnTodayTaskThroughExclamationMark()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("! Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 1, 1, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #12
    [Fact]
    public void ParseTasks_ReturnTomorrowTaskThroughExclamationMark()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("!! Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 1, 2, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #13
    [Fact]
    public void ParseTasks_ReturnDayAfterAfterTomorrowTaskThroughExclamationMark()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("!!!! Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 1, 4, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #14
    [Fact]
    public void ParseTasks_ReturnDayAfterTomorrowNextMonthTaskThroughExclamationMark()
    {
        var service = new TaskParserService(DateServiceMock(2019, 1, 31));

        var result = service.ParseTasks("!!! Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 2, 2, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #15
    [Fact]
    public void ParseTasks_ReturnThisWeekMondayTaskThroughExclamationMark()
    {
        var service = new TaskParserService(DateServiceMock(2019, 7, 31));

        var result = service.ParseTasks("!1 Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 7, 29, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #15.1
    [Fact]
    public void ParseTasks_ReturnNextMondayTaskThroughExclamationMark()
    {
        var service = new TaskParserService(DateServiceMock(2019, 7, 28));

        var result = service.ParseTasks("!!1 Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 7, 29, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #16
    [Fact]
    public void ParseTasks_ReturnNextWednesdayTaskThroughExclamationMark()
    {
        var service = new TaskParserService(DateServiceMock(2019, 7, 28));

        var result = service.ParseTasks("!!3 Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 7, 31, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #17
    [Fact]
    public void ParseTasks_ReturnNextFridayTaskThroughExclamationMark()
    {
        var service = new TaskParserService(DateServiceMock(2019, 7, 28));

        var result = service.ParseTasks("!!5 Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 8, 2, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #17.1
    [Fact]
    public void ParseTasks_ReturnWeekAfterNextMondayTaskThroughExclamationMark()
    {
        var service = new TaskParserService(DateServiceMock(2019, 7, 28));

        var result = service.ParseTasks("!!!1 Test").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 8, 5, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #18
    [Fact]
    public void ParseTasks_ExclamationMark11IsNotWeekShiftPattern()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("!11 Test").Single();

        Assert.Equal("!11 Test", result.Title);
    }

    // #19
    [Fact]
    public void ParseTasks_DateWithExclamation()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("1231! Test").Single();

        Assert.Equal("1231! Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #20
    [Fact]
    public void ParseTasks_ReturnRoutineTaskWithDate()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("0220 Test *").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Routine, result.Type);
        Assert.Equal(new DateTime(2019, 2, 20, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #21
    [Fact]
    public void ParseTasks_ReturnRoutineTaskWithDateAndTime()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("20150220 2359 Test *").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Routine, result.Type);
        Assert.Equal(new DateTime(2015, 2, 20, 0, 0, 0), result.Date);
        Assert.Equal(1439, result.Time);
    }

    // #22
    [Fact]
    public void ParseTasks_RoutineAndProbable()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("Test *?").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Routine, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #22.1
    [Fact]
    public void ParseTasks_ProbableAndRoutine()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("Test ?*").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Routine, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #23
    [Fact]
    public void ParseTasks_RoutineAndAdditional()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("Test !*").Single();

        Assert.Equal("Test !*", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.False(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    [Fact]
    public void ParseTaskTemplate_TaskWithTime_SkipsDate()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTaskTemplate("1010 Test!");

        Assert.Equal("Test!", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Null(result.Date);
        Assert.Equal(610, result.Time);
    }

    // #24
    [Fact]
    public void ParseTasks_ReturnWeeklyTask()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("Test %").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Weekly, result.Type);
        Assert.False(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #25
    [Fact]
    public void ParseTasks_WeeklyAndProbable()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("Test %?").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Weekly, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #25.1
    [Fact]
    public void ParseTasks_ProbableAndWeekly()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("Test ?%").Single();

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Weekly, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #26
    [Fact]
    public void ParseTasks_WeeklyDuplicateCancels()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("Test %%").Single();

        Assert.Equal("Test %%", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.False(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #27
    [Fact]
    public void ParseTasks_WeeklyAndRoutineConflict()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("Test %*").Single();

        Assert.Equal("Test %*", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.False(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #28
    [Fact]
    public void ParseTasks_ProbableWeeklyProbableConflict()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("Test ?%?").Single();

        Assert.Equal("Test ?%?", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.False(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // [Date range] tests #29-#39 are synced 1:1 with FE TaskConvertService range tests.
    // On BE, ParseTask combines parse + validation + expansion, so invalid ranges throw,
    // whereas FE exposes parse / expand / validate as separate methods.

    // #29
    [Fact]
    public void ParseTasks_DateRangeWithoutYear_CreatesTaskPerInclusiveDay()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("0909-0913 venice");

        Assert.Equal(5, result.Count);
        Assert.Equal(new DateTime(2019, 9, 9), result[0].Date);
        Assert.Equal(new DateTime(2019, 9, 13), result[4].Date);
        Assert.All(result, x => Assert.Equal("venice", x.Title));
    }

    // #30
    [Fact]
    public void ParseTasks_DateRangeWithTimeAndType_AppliesToEveryTask()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("0909-0913 1012 venice !");

        Assert.Equal(5, result.Count);
        Assert.Equal(new DateTime(2019, 9, 9), result[0].Date);
        Assert.Equal(new DateTime(2019, 9, 13), result[4].Date);
        Assert.All(result, x =>
        {
            Assert.Equal("venice", x.Title);
            Assert.Equal(TaskTypeDto.Additional, x.Type);
            Assert.Equal(612, x.Time);
        });
    }

    // #31
    [Fact]
    public void ParseTasks_DateRangeWithYearOnBothEndpoints_ParsesYears()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("20270909-20270915 venice");

        Assert.Equal(7, result.Count);
        Assert.Equal(new DateTime(2027, 9, 9), result[0].Date);
        Assert.Equal(new DateTime(2027, 9, 15), result[6].Date);
    }

    // #32
    [Fact]
    public void ParseTasks_DateRangeWithMixedYearEndpoints_ParsesEachEndpointIndependently()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("0909-20190915 venice");

        Assert.Equal(7, result.Count);
        Assert.Equal(new DateTime(2019, 9, 9), result[0].Date);
        Assert.Equal(new DateTime(2019, 9, 15), result[6].Date);
    }

    // #33
    [Fact]
    public void ParseTasks_DateRangeWithoutSpace_NotTreatedAsRange()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("0909-0913venice");

        Assert.Single(result);
        Assert.Equal("0909-0913venice", result[0].Title);
        Assert.Null(result[0].Date);
    }

    // #34
    [Fact]
    public void ParseTasks_SingleDayRange_Throws()
    {
        var service = new TaskParserService(DateServiceMock());

        Assert.Throws<ServiceException>(() => service.ParseTasks("0909-0909 venice"));
    }

    // #35
    [Fact]
    public void ParseTasks_DateRangeAtLimit_CreatesTasks()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("0901-1001 venice");

        Assert.Equal(31, result.Count);
        Assert.Equal(new DateTime(2019, 9, 1), result[0].Date);
        Assert.Equal(new DateTime(2019, 10, 1), result[30].Date);
    }

    // #36
    [Fact]
    public void ParseTasks_DateRangeExceedingLimit_Throws()
    {
        var service = new TaskParserService(DateServiceMock());

        Assert.Throws<ServiceException>(() => service.ParseTasks("0901-1002 venice"));
    }

    // #37
    [Fact]
    public void ParseTasks_MultiYearDateRange_Throws()
    {
        var service = new TaskParserService(DateServiceMock());

        Assert.Throws<ServiceException>(() => service.ParseTasks("20270909-20280913 venice"));
    }

    // #38
    [Fact]
    public void ParseTasks_ReversedDateRange_Throws()
    {
        var service = new TaskParserService(DateServiceMock());

        Assert.Throws<ServiceException>(() => service.ParseTasks("0913-0909 venice"));
    }

    // #39
    [Fact]
    public void ParseTasks_MinimumValidRange_CreatesTwoTasks()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTasks("0910-0911 venice");

        Assert.Equal(2, result.Count);
        Assert.Equal(new DateTime(2019, 9, 10), result[0].Date);
        Assert.Equal(new DateTime(2019, 9, 11), result[1].Date);
    }

    private static IDateService DateServiceMock(int year = 2019, int month = 1, int date = 1)
    {
        var mock = new Mock<IDateService>();
        mock.SetupGet(x => x.Today).Returns(new DateTime(year, month, date));
        return mock.Object;
    }
}
