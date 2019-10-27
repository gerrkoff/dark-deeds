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
            var service = new RecurrenceCreatorService(null, null, null, dateMock.Object, null);

            var (_, result) = service.EvaluatePeriod();
            

            Assert.Equal(new DateTime(2019, 9, 16), result);
        }
        
        [Fact]
        public void EvaluatePeriod_ShouldCorrectlyEvaluateEndDateOnTheEndOfTheWeek()
        {
            var dateMock = new Mock<IDateService>();
            dateMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 1));
            var service = new RecurrenceCreatorService(null, null, null, dateMock.Object, null);

            var (_, result) = service.EvaluatePeriod();

            Assert.Equal(new DateTime(2019, 9, 9), result);
        }
        
        [Fact]
        public void EvaluatePeriod_ShouldAdd12HoursToEndDate()
        {
            var dateMock = new Mock<IDateService>();
            dateMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 1, 23, 0, 0));
            var service = new RecurrenceCreatorService(null, null, null, dateMock.Object, null);

            var (_, result) = service.EvaluatePeriod();

            Assert.Equal(new DateTime(2019, 9, 16), result);
        }
        
        [Fact]
        public void EvaluatePeriod_ShouldCorrectlyEvaluateStartDate()
        {
            var dateMock = new Mock<IDateService>();
            dateMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 3, 11, 59, 59));
            var service = new RecurrenceCreatorService(null, null, null, dateMock.Object, null);

            var (result, _) = service.EvaluatePeriod();

            Assert.Equal(new DateTime(2019, 9, 2), result);
        }
    }
}