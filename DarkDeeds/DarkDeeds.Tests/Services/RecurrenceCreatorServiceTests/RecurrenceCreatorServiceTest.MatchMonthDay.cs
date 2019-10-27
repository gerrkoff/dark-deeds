using System;
using DarkDeeds.Data.Entity;
using DarkDeeds.Services.Implementation;
using Xunit;

namespace DarkDeeds.Tests.Services.RecurrenceCreatorServiceTests
{
    public partial class RecurrenceCreatorServiceTest
    { 
        [Fact]
        public void MatchMonthDay_MatchIfEmpty()
        {
            var service = new RecurrenceCreatorService(null, null, null, null, null);

            var result = service.MatchMonthDay(new PlannedRecurrenceEntity(), new DateTime());

            Assert.True(result);
        }
        
        [Fact]
        public void MatchMonthDay_MatchOneDate()
        {
            var service = new RecurrenceCreatorService(null, null, null, null, null);

            var result = service.MatchMonthDay(new PlannedRecurrenceEntity
            {
                EveryMonthDay = "13"
            }, new DateTime(2019, 9, 13));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchMonthDay_MatchOneOfSeveralDates()
        {
            var service = new RecurrenceCreatorService(null, null, null, null, null);

            var result = service.MatchMonthDay(new PlannedRecurrenceEntity
            {
                EveryMonthDay = "13,15,22"
            }, new DateTime(2019, 9, 13));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchMonthDay_CountBigDateAsLastDayOfMonth()
        {
            var service = new RecurrenceCreatorService(null, null, null, null, null);

            var result = service.MatchMonthDay(new PlannedRecurrenceEntity
            {
                EveryMonthDay = "13,15,22,40"
            }, new DateTime(2020, 2, 29));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchMonthDay_DoNotMatchAnyDateInList()
        {
            var service = new RecurrenceCreatorService(null, null, null, null, null);

            var result = service.MatchMonthDay(new PlannedRecurrenceEntity
            {
                EveryMonthDay = "13,15,22,40"
            }, new DateTime(2020, 2, 16));

            Assert.False(result);
        }
    }
}