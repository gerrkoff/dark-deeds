using System;
using DarkDeeds.Data.Entity;
using DarkDeeds.Enums;
using DarkDeeds.Services.Implementation;
using Xunit;

namespace DarkDeeds.Tests.Services
{
    public partial class RecurrenceServiceTest
    { 
        [Fact]
        public void MatchWeekday_MatchOneWeekday()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchWeekday(new PlannedRecurrenceEntity
            {
                EveryWeekday = RecurrenceWeekdayEnum.Monday
            }, new DateTime(2019, 9, 2));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchWeekday_MatchOneOfTwoWeekdays()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchWeekday(new PlannedRecurrenceEntity
            {
                EveryWeekday = RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Friday
            }, new DateTime(2019, 9, 6));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchWeekday_DoNotMatchOneOfWeekdays()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchWeekday(new PlannedRecurrenceEntity
            {
                EveryWeekday = RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Friday
            }, new DateTime(2019, 9, 5));

            Assert.False(result);
        }
        
        [Fact]
        public void MatchWeekday_MatchIfWeekdayIsNull()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchWeekday(new PlannedRecurrenceEntity
            {
                EveryWeekday = null
            }, new DateTime());

            Assert.True(result);
        }
    }
}