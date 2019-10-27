using System;
using DarkDeeds.Data.Entity;
using DarkDeeds.Services.Implementation;
using Xunit;

namespace DarkDeeds.Tests.Services
{
    public partial class RecurrenceServiceTest
    { 
        [Fact]
        public void MatchMonthDays_MatchIfEmpty()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchMonthDays(new PlannedRecurrenceEntity(), new DateTime());

            Assert.True(result);
        }
        
        [Fact]
        public void MatchMonthDays_MatchOneDate()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchMonthDays(new PlannedRecurrenceEntity
            {
                EveryMonthDay = "13"
            }, new DateTime(2019, 9, 13));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchMonthDays_MatchOneOfSeveralDates()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchMonthDays(new PlannedRecurrenceEntity
            {
                EveryMonthDay = "13,15,22"
            }, new DateTime(2019, 9, 13));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchMonthDays_CountBigDateAsLastDayOfMonth()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchMonthDays(new PlannedRecurrenceEntity
            {
                EveryMonthDay = "13,15,22,40"
            }, new DateTime(2020, 2, 29));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchMonthDays_DoNotMatchAnyDateInList()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchMonthDays(new PlannedRecurrenceEntity
            {
                EveryMonthDay = "13,15,22,40"
            }, new DateTime(2020, 2, 16));

            Assert.False(result);
        }
    }
}