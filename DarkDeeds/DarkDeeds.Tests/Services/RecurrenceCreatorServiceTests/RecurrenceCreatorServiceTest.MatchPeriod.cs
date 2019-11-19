using System;
using DarkDeeds.Data.Entity;
using DarkDeeds.Services.Implementation;
using Xunit;

namespace DarkDeeds.Tests.Services.RecurrenceCreatorServiceTests
{
    public partial class RecurrenceCreatorServiceTest
    { 
        [Fact]
        public void MatchPeriod_ShouldMatchIfWithidPeriod()
        {
            var service = new RecurrenceCreatorService(null, null, null, null, null, null, null);

            var result = service.MatchPeriod(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EndDate =  new DateTime(2019, 9, 6)
            }, new DateTime(2019, 9, 5));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchPeriod_ShouldMatchIfEqualsToStartDate()
        {
            var service = new RecurrenceCreatorService(null, null, null, null, null, null, null);

            var result = service.MatchPeriod(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EndDate =  new DateTime(2019, 9, 6)
            }, new DateTime(2019, 9, 4));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchPeriod_ShouldMatchIfEqualsToEndDate()
        {
            var service = new RecurrenceCreatorService(null, null, null, null, null, null, null);

            var result = service.MatchPeriod(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4),
                EndDate =  new DateTime(2019, 9, 6)
            }, new DateTime(2019, 9, 6));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchPeriod_ShouldMatchEvenIfEndDateIsNull()
        {
            var service = new RecurrenceCreatorService(null, null, null, null, null, null, null);

            var result = service.MatchPeriod(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4)
            }, new DateTime(2019, 12, 31));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchPeriod_ShouldNotMatchIfLessThanStartDate()
        {
            var service = new RecurrenceCreatorService(null, null, null, null, null, null, null);

            var result = service.MatchPeriod(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4)
            }, new DateTime(2019, 9, 3));

            Assert.False(result);
        }
    }
}