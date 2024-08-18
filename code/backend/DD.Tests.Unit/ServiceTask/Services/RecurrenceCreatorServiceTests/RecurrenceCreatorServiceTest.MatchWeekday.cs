using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Entities.Enums;
using Xunit;

namespace DD.Tests.Unit.ServiceTask.Services.RecurrenceCreatorServiceTests;

public partial class RecurrenceCreatorServiceTest
{
    [Fact]
    public void MatchWeekday_ShouldMatchOneWeekday()
    {
        var result = DD.ServiceTask.Domain.Services.RecurrenceCreatorService.MatchWeekday(
            new PlannedRecurrenceEntity
            {
                EveryWeekday = RecurrenceWeekday.Monday,
            },
            new DateTime(2019, 9, 2));

        Assert.True(result);
    }

    [Fact]
    public void MatchWeekday_ShouldMatchOneOfTwoWeekdays()
    {
        var result = DD.ServiceTask.Domain.Services.RecurrenceCreatorService.MatchWeekday(
            new PlannedRecurrenceEntity
            {
                EveryWeekday = RecurrenceWeekday.Monday | RecurrenceWeekday.Friday,
            },
            new DateTime(2019, 9, 6));

        Assert.True(result);
    }

    [Fact]
    public void MatchWeekday_ShouldNotMatchOneOfWeekdays()
    {
        var result = DD.ServiceTask.Domain.Services.RecurrenceCreatorService.MatchWeekday(
            new PlannedRecurrenceEntity
            {
                EveryWeekday = RecurrenceWeekday.Monday | RecurrenceWeekday.Friday,
            },
            new DateTime(2019, 9, 5));

        Assert.False(result);
    }

    [Fact]
    public void MatchWeekday_ShouldMatchIfWeekdayIsNull()
    {
        var result = DD.ServiceTask.Domain.Services.RecurrenceCreatorService.MatchWeekday(
            new PlannedRecurrenceEntity
            {
                EveryWeekday = null,
            },
            new DateTime());

        Assert.True(result);
    }
}
