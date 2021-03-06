using System;
using DarkDeeds.Services.Implementation;
using DarkDeeds.Services.Interface;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.Services.RecurrenceCreatorServiceTests
{
    public partial class RecurrenceCreatorServiceTest
    {
        [Fact]
        public void EvaluatePeriod_ShouldCorrectlyEvaluateEndDate()
        {
            var dateMock = new Mock<IDateService>();
            dateMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 2));
            var service = new RecurrenceCreatorService(null, null, null, dateMock.Object, null, null, null);

            var (_, result) = service.EvaluatePeriod(0);
            

            Assert.Equal(new DateTime(2019, 9, 16), result);
        }
        
        [Fact]
        public void EvaluatePeriod_ShouldCorrectlyEvaluateEndDateOnTheEndOfTheWeek()
        {
            var dateMock = new Mock<IDateService>();
            dateMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 1));
            var service = new RecurrenceCreatorService(null, null, null, dateMock.Object, null, null, null);

            var (_, result) = service.EvaluatePeriod(0);

            Assert.Equal(new DateTime(2019, 9, 9), result);
        }
        
        [Fact]
        public void EvaluatePeriod_ShouldConsiderTimezoneOffset()
        {
            var dateMock = new Mock<IDateService>();
            dateMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 1, 23, 59, 0));
            var service = new RecurrenceCreatorService(null, null, null, dateMock.Object, null, null, null);

            var (resultStart, resultEnd) = service.EvaluatePeriod(1);

            Assert.Equal(new DateTime(2019, 9, 2), resultStart);
            Assert.Equal(new DateTime(2019, 9, 16), resultEnd);
        }
    }
}