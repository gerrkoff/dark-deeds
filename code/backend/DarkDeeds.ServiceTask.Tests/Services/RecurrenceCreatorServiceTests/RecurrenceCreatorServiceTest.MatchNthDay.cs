using System;
using DD.TaskService.Domain.Entities;
using Xunit;

namespace DarkDeeds.ServiceTask.Tests.Services.RecurrenceCreatorServiceTests;

public partial class RecurrenceCreatorServiceTest
{
    [Fact]
    public void MatchNthDay_FirstDayAlwaysCounts()
    {
        var service = Service();

        var result = service.MatchNthDay(new PlannedRecurrenceEntity
        {
            StartDate = new DateTime(2019, 9, 4),
            EveryNthDay = 1000,
        }, new DateTime(2019, 9, 4));

        Assert.True(result);
    }

    [Fact]
    public void MatchNthDay_ShouldMatchNextDayAfterStartDateWith1Number()
    {
        var service = Service();

        var result = service.MatchNthDay(new PlannedRecurrenceEntity
        {
            StartDate = new DateTime(2019, 9, 4),
            EveryNthDay = 1,
        }, new DateTime(2019, 9, 5));

        Assert.True(result);
    }

    [Fact]
    public void MatchNthDay_ShouldNotMatchNextDayAfterStartDateWith2Number()
    {
        var service = Service();

        var result = service.MatchNthDay(new PlannedRecurrenceEntity
        {
            StartDate = new DateTime(2019, 9, 4),
            EveryNthDay = 2,
        }, new DateTime(2019, 9, 5));

        Assert.False(result);
    }

    [Fact]
    public void MatchNthDay_ShouldMatch2NextDaysAfterStarDateWith2Number()
    {
        var service = Service();

        var result = service.MatchNthDay(new PlannedRecurrenceEntity
        {
            StartDate = new DateTime(2019, 9, 4),
            EveryNthDay = 2,
        }, new DateTime(2019, 9, 6));

        Assert.True(result);
    }

    [Fact]
    public void MatchNthDay_ShouldMatch5NextDaysAfterStartDateWith5Number()
    {
        var service = Service();

        var result = service.MatchNthDay(new PlannedRecurrenceEntity
        {
            StartDate = new DateTime(2019, 9, 4),
            EveryNthDay = 5,
        }, new DateTime(2019, 9, 9));

        Assert.True(result);
    }

    [Fact]
    public void MatchNthDay_ShouldNotMatch4NextDayAfterStartDateWith5Number()
    {
        var service = Service();

        var result = service.MatchNthDay(new PlannedRecurrenceEntity
        {
            StartDate = new DateTime(2019, 9, 4),
            EveryNthDay = 4,
        }, new DateTime(2019, 9, 9));

        Assert.False(result);
    }

    [Fact]
    public void MatchNthDay_ShouldMatch5NextDayAfterStartDateWith5NumberWithMonthChange()
    {
        var service = Service();

        var result = service.MatchNthDay(new PlannedRecurrenceEntity
        {
            StartDate = new DateTime(2019, 9, 27),
            EveryNthDay = 4,
        }, new DateTime(2019, 10, 1));

        Assert.True(result);
    }
}
