using System;
using Xunit;

namespace DarkDeeds.ServiceTask.Tests.Services.RecurrenceCreatorServiceTests;

public partial class RecurrenceCreatorServiceTest
{
    [Fact]
    public void EvaluatePeriod_ShouldCorrectlyEvaluateEndDate()
    {
        _dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 2));

        var service = Service();

        var (_, result) = service.EvaluatePeriod(0);


        Assert.Equal(new DateTime(2019, 9, 16), result);
    }

    [Fact]
    public void EvaluatePeriod_ShouldCorrectlyEvaluateEndDateOnTheEndOfTheWeek()
    {
        _dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 1));

        var service = Service();

        var (_, result) = service.EvaluatePeriod(0);

        Assert.Equal(new DateTime(2019, 9, 9), result);
    }

    [Fact]
    public void EvaluatePeriod_ShouldConsiderTimezoneOffset()
    {
        _dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 1, 23, 59, 0));

        var service = Service();

        var (resultStart, resultEnd) = service.EvaluatePeriod(1);

        Assert.Equal(new DateTime(2019, 9, 2), resultStart);
        Assert.Equal(new DateTime(2019, 9, 16), resultEnd);
    }
}
