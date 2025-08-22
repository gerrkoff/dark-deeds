using System.Diagnostics.CodeAnalysis;
using DD.ServiceTask.Domain.Services;
using DD.Shared.Details.Abstractions.Dto;
using Moq;
using Xunit;

namespace DD.Tests.Unit.ServiceTask.Services;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Tests")]
public class TaskParserServiceTest : BaseTest
{
    // These tests should be synced with FE TaskConverter.convertStringToModel tests

    // #1
    [Fact]
    public void ParseTask_ReturnTaskWithNoDateAndNoTime()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("Test!");

        Assert.Equal("Test!", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #2
    [Fact]
    public void ParseTask_ReturnTaskWithDateAndNoTime()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("1231 Test!");

        Assert.Equal("Test!", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 12, 31, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #3
    [Fact]
    public void ParseTask_ReturnTaskWithDateAndNoTime_NotWorkingWithoutSpace()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("0101Test!!!");

        Assert.Equal("0101Test!!!", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #4
    [Fact]
    public void ParseTask_ReturnTaskWithDateAndTime()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("1231 2359 Test!");

        Assert.Equal("Test!", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 12, 31, 0, 0, 0), result.Date);
        Assert.Equal(1439, result.Time);
    }

    // #5
    [Fact]
    public void ParseTask_ReturnTaskWithDateAndTime_NotWorkingWithoutSpace()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("0101 0101Test!!!");

        Assert.Equal("0101Test!!!", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 1, 1, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #6
    [Fact]
    public void ParseTask_ReturnTaskWithDateAndNoTimeWithYear()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("20170101 Test");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2017, 1, 1, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #7
    [Fact]
    public void ParseTask_ReturnProbableTask()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("Test! ?");

        Assert.Equal("Test!", result.Title);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #8
    [Fact]
    public void ParseTask_ReturnAdditionalTaskWithDate()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("0220 Test !");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Additional, result.Type);
        Assert.Equal(new DateTime(2019, 2, 20, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #9
    [Fact]
    public void ParseTask_ReturnAdditionalTaskWithDateAndTime()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("20150220 2359 Test !");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Additional, result.Type);
        Assert.Equal(new DateTime(2015, 2, 20, 0, 0, 0), result.Date);
        Assert.Equal(1439, result.Time);
    }

    // #10
    [Fact]
    public void ParseTask_AdditionalAndProbable()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("Test !?");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Additional, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #10.1
    [Fact]
    public void ParseTask_ProbableAndAdditional()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("Test ?!");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Additional, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #11
    [Fact]
    public void ParseTask_ReturnTodayTaskThroughExclamationMark()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("! Test");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 1, 1, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #12
    [Fact]
    public void ParseTask_ReturnTomorrowTaskThroughExclamationMark()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("!! Test");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 1, 2, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #13
    [Fact]
    public void ParseTask_ReturnDayAfterAfterTomorrowTaskThroughExclamationMark()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("!!!! Test");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 1, 4, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #14
    [Fact]
    public void ParseTask_ReturnDayAfterTomorrowNextMonthTaskThroughExclamationMark()
    {
        var service = new TaskParserService(DateServiceMock(2019, 1, 31));

        var result = service.ParseTask("!!! Test");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 2, 2, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #15
    [Fact]
    public void ParseTask_ReturnNextMondayTaskThroughExclamationMark()
    {
        var service = new TaskParserService(DateServiceMock(2019, 7, 28));

        var result = service.ParseTask("!1 Test");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 7, 29, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #16
    [Fact]
    public void ParseTask_ReturnNextWednesdayTaskThroughExclamationMark()
    {
        var service = new TaskParserService(DateServiceMock(2019, 7, 28));

        var result = service.ParseTask("!3 Test");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 7, 31, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #17
    [Fact]
    public void ParseTask_ReturnNextFridayTaskThroughExclamationMark()
    {
        var service = new TaskParserService(DateServiceMock(2019, 7, 28));

        var result = service.ParseTask("!5 Test");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Equal(new DateTime(2019, 8, 2, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #18
    [Fact]
    public void ParseTask_ExclamationMark11IsNotWeekShiftPattern()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("!11 Test");

        Assert.Equal("!11 Test", result.Title);
    }

    // #19
    [Fact]
    public void ParseTask_DateWithExclamation()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("1231! Test");

        Assert.Equal("1231! Test", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #20
    [Fact]
    public void ParseTask_ReturnRoutineTaskWithDate()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("0220 Test *");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Routine, result.Type);
        Assert.Equal(new DateTime(2019, 2, 20, 0, 0, 0), result.Date);
        Assert.Null(result.Time);
    }

    // #21
    [Fact]
    public void ParseTask_ReturnRoutineTaskWithDateAndTime()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("20150220 2359 Test *");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Routine, result.Type);
        Assert.Equal(new DateTime(2015, 2, 20, 0, 0, 0), result.Date);
        Assert.Equal(1439, result.Time);
    }

    // #22
    [Fact]
    public void ParseTask_RoutineAndProbable()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("Test *?");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Routine, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #22.1
    [Fact]
    public void ParseTask_ProbableAndRoutine()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("Test ?*");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Routine, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #23
    [Fact]
    public void ParseTask_RoutineAndAdditional()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("Test !*");

        Assert.Equal("Test !*", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.False(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    [Fact]
    public void ParseTask_IgnoreDateTaskWithTime()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("1010 Test!", ignoreDate: true);

        Assert.Equal("Test!", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.Null(result.Date);
        Assert.Equal(610, result.Time);
    }

    // #24
    [Fact]
    public void ParseTask_ReturnWeeklyTask()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("Test %");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Weekly, result.Type);
        Assert.False(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #25
    [Fact]
    public void ParseTask_WeeklyAndProbable()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("Test %?");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Weekly, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #25.1
    [Fact]
    public void ParseTask_ProbableAndWeekly()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("Test ?%");

        Assert.Equal("Test", result.Title);
        Assert.Equal(TaskTypeDto.Weekly, result.Type);
        Assert.True(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #26
    [Fact]
    public void ParseTask_WeeklyDuplicateCancels()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("Test %%");

        Assert.Equal("Test %%", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.False(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #27
    [Fact]
    public void ParseTask_WeeklyAndRoutineConflict()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("Test %*");

        Assert.Equal("Test %*", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.False(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    // #28
    [Fact]
    public void ParseTask_ProbableWeeklyProbableConflict()
    {
        var service = new TaskParserService(DateServiceMock());

        var result = service.ParseTask("Test ?%?");

        Assert.Equal("Test ?%?", result.Title);
        Assert.Equal(TaskTypeDto.Simple, result.Type);
        Assert.False(result.IsProbable);
        Assert.Null(result.Date);
        Assert.Null(result.Time);
    }

    private static IDateService DateServiceMock(int year = 2019, int month = 1, int date = 1)
    {
        var mock = new Mock<IDateService>();
        mock.SetupGet(x => x.Today).Returns(new DateTime(year, month, date));
        return mock.Object;
    }
}
