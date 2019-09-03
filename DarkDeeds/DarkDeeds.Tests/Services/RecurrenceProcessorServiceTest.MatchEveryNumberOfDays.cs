using System;
using DarkDeeds.Data.Entity;
using DarkDeeds.Services.Implementation;
using Xunit;

namespace DarkDeeds.Tests.Services
{
    public partial class RecurrenceServiceTest
    { 
        [Fact]
        public void MatchEveryNumberOfDays_DoNotMatchIfDateIsBeforeStartDate()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchEveryNumberOfDays(new RecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4)
            }, new DateTime(2019, 9, 3));

            Assert.False(result);
        }
        
        [Fact]
        public void MatchEveryNumberOfDays_MatchIfDateTimeIsBeforeStartDateButSameDay()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchEveryNumberOfDays(new RecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4, 10, 0, 0),
                EveryNumberOfDays = 1,
            }, new DateTime(2019, 9, 4, 9, 0, 0));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchEveryNumberOfDays_FirstDayAlwaysCounts()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchEveryNumberOfDays(new RecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EveryNumberOfDays = 1000,
            }, new DateTime(2019, 9, 4));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchEveryNumberOfDays_MatchNextDayAfterStartDateWith1Number()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchEveryNumberOfDays(new RecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EveryNumberOfDays = 1,
            }, new DateTime(2019, 9, 5));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchEveryNumberOfDays_DoNotMatchNextDayAfterStartDateWith2Number()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchEveryNumberOfDays(new RecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EveryNumberOfDays = 2,
            }, new DateTime(2019, 9, 5));

            Assert.False(result);
        }
        
        [Fact]
        public void MatchEveryNumberOfDays_Match2NextDaysAfterStarDatetWith2Number()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchEveryNumberOfDays(new RecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EveryNumberOfDays = 2,
            }, new DateTime(2019, 9, 6));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchEveryNumberOfDays_Match5NextDaysAfterStartDateWith5Number()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchEveryNumberOfDays(new RecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EveryNumberOfDays = 5,
            }, new DateTime(2019, 9, 9));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchEveryNumberOfDays_DoNotMatch4NextDayAfterStartDateWith5Number()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchEveryNumberOfDays(new RecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EveryNumberOfDays = 4,
            }, new DateTime(2019, 9, 9));

            Assert.False(result);
        }
        
        [Fact]
        public void MatchEveryNumberOfDays_Match5NextDayAfterStartDateWith5NumberWithMonthChange()
        {
            var service = new RecurrenceProcessorService(null, null, null, null);

            var result = service.MatchEveryNumberOfDays(new RecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 27),
                EveryNumberOfDays = 4,
            }, new DateTime(2019, 10, 1));

            Assert.True(result);
        }
    }
}