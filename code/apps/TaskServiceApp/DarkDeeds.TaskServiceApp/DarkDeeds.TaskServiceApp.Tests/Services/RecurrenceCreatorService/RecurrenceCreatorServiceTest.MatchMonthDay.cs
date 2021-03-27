using System;
using DarkDeeds.TaskServiceApp.Entities.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DarkDeeds.TaskServiceApp.Tests.Services.RecurrenceCreatorService
{
    public partial class RecurrenceCreatorServiceTest
    { 
        [Fact]
        public void MatchMonthDay_ShouldMatchIfEmpty()
        {
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(null, null, null, null, null, null, null);

            var result = service.MatchMonthDay(new PlannedRecurrenceEntity(), new DateTime());

            Assert.True(result);
        }
        
        [Fact]
        public void MatchMonthDay_ShouldMatchOneDate()
        {
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(null, null, null, null, null, null, null);

            var result = service.MatchMonthDay(new PlannedRecurrenceEntity
            {
                EveryMonthDay = "13"
            }, new DateTime(2019, 9, 13));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchMonthDay_ShouldMatchOneOfSeveralDates()
        {
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(null, null, null, null, null, null, null);

            var result = service.MatchMonthDay(new PlannedRecurrenceEntity
            {
                EveryMonthDay = "13,15,22"
            }, new DateTime(2019, 9, 13));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchMonthDay_CountBigDateAsLastDayOfMonth()
        {
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(null, null, null, null, null, null, null);

            var result = service.MatchMonthDay(new PlannedRecurrenceEntity
            {
                EveryMonthDay = "13,15,22,40"
            }, new DateTime(2020, 2, 29));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchMonthDay_ShouldNotMatchAnyDateInList()
        {
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(null, null, null, null, null, null, null);

            var result = service.MatchMonthDay(new PlannedRecurrenceEntity
            {
                EveryMonthDay = "13,15,22,40"
            }, new DateTime(2020, 2, 16));

            Assert.False(result);
        }
        
        [Fact]
        public void MatchMonthDay_ShouldMatchIfCanNotParseMonthDays_FormatException()
        {
            var logger = new Mock<ILogger<TaskServiceApp.Services.Implementation.RecurrenceCreatorService>>().Object;
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(null, null, null, null, logger, null, null);

            var result = service.MatchMonthDay(new PlannedRecurrenceEntity
            {
                EveryMonthDay = "1q3"
            }, new DateTime(2019, 9, 13));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchMonthDay_ShouldMatchIfCanNotParseMonthDays_OverflowException()
        {
            var logger = new Mock<ILogger<TaskServiceApp.Services.Implementation.RecurrenceCreatorService>>().Object;
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(null, null, null, null, logger, null, null);

            var result = service.MatchMonthDay(new PlannedRecurrenceEntity
            {
                EveryMonthDay = "999999999999999999"
            }, new DateTime(2019, 9, 13));

            Assert.True(result);
        }
    }
}