using DD.ServiceTask.Domain.Entities;
using Xunit;

namespace DD.Tests.Unit.ServiceTask.Services.RecurrenceCreatorServiceTests;

public partial class RecurrenceCreatorServiceTest
{
    [Fact]
    public void MatchPeriod_ShouldMatchIfWithinPeriod()
    {
        var result = DD.ServiceTask.Domain.Services.RecurrenceCreatorService.MatchPeriod(
            new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EndDate = new DateTime(2019, 9, 6),
            },
            new DateTime(2019, 9, 5));

        Assert.True(result);
    }

    [Fact]
    public void MatchPeriod_ShouldMatchIfEqualsToStartDate()
    {
        var result = DD.ServiceTask.Domain.Services.RecurrenceCreatorService.MatchPeriod(
            new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EndDate = new DateTime(2019, 9, 6),
            },
            new DateTime(2019, 9, 4));

        Assert.True(result);
    }

    [Fact]
    public void MatchPeriod_ShouldMatchIfEqualsToEndDate()
    {
        var result = DD.ServiceTask.Domain.Services.RecurrenceCreatorService.MatchPeriod(
            new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EndDate = new DateTime(2019, 9, 6),
            },
            new DateTime(2019, 9, 6));

        Assert.True(result);
    }

    [Fact]
    public void MatchPeriod_ShouldMatchEvenIfEndDateIsNull()
    {
        var result = DD.ServiceTask.Domain.Services.RecurrenceCreatorService.MatchPeriod(
            new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
            },
            new DateTime(2019, 12, 31));

        Assert.True(result);
    }

    [Fact]
    public void MatchPeriod_ShouldNotMatchIfLessThanStartDate()
    {
        var result = DD.ServiceTask.Domain.Services.RecurrenceCreatorService.MatchPeriod(
            new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
            },
            new DateTime(2019, 9, 3));

        Assert.False(result);
    }
}
