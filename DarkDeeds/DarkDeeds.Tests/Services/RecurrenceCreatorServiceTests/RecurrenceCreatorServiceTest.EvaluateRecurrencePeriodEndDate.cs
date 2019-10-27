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
        public void EvaluateRecurrencePeriodEndDate_CorrectlyEvaluate()
        {
            var dateMock = new Mock<IDateService>();
            dateMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 2, 10, 0, 0));
            var service = new RecurrenceCreatorService(null, null, null, dateMock.Object);

            var result = service.EvaluateRecurrencePeriodEndDate();

            Assert.Equal(new DateTime(2019, 9, 16, 10, 0, 0), result);
        }
        
        [Fact]
        public void EvaluateRecurrencePeriodEndDate_CorrectlyEvaluateOnTheEndOfTheWeek()
        {
            var dateMock = new Mock<IDateService>();
            dateMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 1, 10, 0, 0));
            var service = new RecurrenceCreatorService(null, null, null, dateMock.Object);

            var result = service.EvaluateRecurrencePeriodEndDate();

            Assert.Equal(new DateTime(2019, 9, 16, 10, 0, 0), result);
        }
    }
}