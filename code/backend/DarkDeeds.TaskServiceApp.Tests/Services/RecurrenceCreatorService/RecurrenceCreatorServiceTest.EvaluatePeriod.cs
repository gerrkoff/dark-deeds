using System;
using DarkDeeds.TaskServiceApp.Services.Interface;
using Moq;
using Xunit;

namespace DarkDeeds.TaskServiceApp.Tests.Services.RecurrenceCreatorService
{
    public partial class RecurrenceCreatorServiceTest
    {
        [Fact]
        public void EvaluatePeriod_ShouldCorrectlyEvaluateEndDate()
        {
            var dateMock = new Mock<IDateService>();
            dateMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 2));
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(null, null, null, dateMock.Object, null, null, null, Mapper);

            var (_, result) = service.EvaluatePeriod(0);
            

            Assert.Equal(new DateTime(2019, 9, 16), result);
        }
        
        [Fact]
        public void EvaluatePeriod_ShouldCorrectlyEvaluateEndDateOnTheEndOfTheWeek()
        {
            var dateMock = new Mock<IDateService>();
            dateMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 1));
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(null, null, null, dateMock.Object, null, null, null, Mapper);

            var (_, result) = service.EvaluatePeriod(0);

            Assert.Equal(new DateTime(2019, 9, 9), result);
        }
        
        [Fact]
        public void EvaluatePeriod_ShouldConsiderTimezoneOffset()
        {
            var dateMock = new Mock<IDateService>();
            dateMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 1, 23, 59, 0));
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(null, null, null, dateMock.Object, null, null, null, Mapper);

            var (resultStart, resultEnd) = service.EvaluatePeriod(1);

            Assert.Equal(new DateTime(2019, 9, 2), resultStart);
            Assert.Equal(new DateTime(2019, 9, 16), resultEnd);
        }
    }
}