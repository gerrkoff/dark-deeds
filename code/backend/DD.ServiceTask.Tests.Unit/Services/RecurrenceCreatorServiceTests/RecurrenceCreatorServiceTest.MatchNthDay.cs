using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Services;
using Xunit;

namespace DD.ServiceTask.Tests.Unit.Services.RecurrenceCreatorServiceTests;

public partial class RecurrenceCreatorServiceTest
{
    [Fact]
    public void MatchNthDay_FirstDayAlwaysCounts()
    {
        var result = RecurrenceCreatorService.MatchNthDay(
            new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EveryNthDay = 1000,
            },
            new DateTime(2019, 9, 4));

        Assert.True(result);
    }

    [Fact]
    public void MatchNthDay_ShouldMatchNextDayAfterStartDateWith1Number()
    {
        var result = RecurrenceCreatorService.MatchNthDay(
            new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EveryNthDay = 1,
            },
            new DateTime(2019, 9, 5));

        Assert.True(result);
    }

    [Fact]
    public void MatchNthDay_ShouldNotMatchNextDayAfterStartDateWith2Number()
    {
        var result = RecurrenceCreatorService.MatchNthDay(
            new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EveryNthDay = 2,
            },
            new DateTime(2019, 9, 5));

        Assert.False(result);
    }

    [Fact]
    public void MatchNthDay_ShouldMatch2NextDaysAfterStarDateWith2Number()
    {
        var result = RecurrenceCreatorService.MatchNthDay(
            new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EveryNthDay = 2,
            },
            new DateTime(2019, 9, 6));

        Assert.True(result);
    }

    [Fact]
    public void MatchNthDay_ShouldMatch5NextDaysAfterStartDateWith5Number()
    {
        var result = RecurrenceCreatorService.MatchNthDay(
            new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EveryNthDay = 5,
            },
            new DateTime(2019, 9, 9));

        Assert.True(result);
    }

    [Fact]
    public void MatchNthDay_ShouldNotMatch4NextDayAfterStartDateWith5Number()
    {
        var result = RecurrenceCreatorService.MatchNthDay(
            new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EveryNthDay = 4,
            },
            new DateTime(2019, 9, 9));

        Assert.False(result);
    }

    [Fact]
    public void MatchNthDay_ShouldMatch5NextDayAfterStartDateWith5NumberWithMonthChange()
    {
        var result = RecurrenceCreatorService.MatchNthDay(
            new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 27),
                EveryNthDay = 4,
            },
            new DateTime(2019, 10, 1));

        Assert.True(result);
    }
}
