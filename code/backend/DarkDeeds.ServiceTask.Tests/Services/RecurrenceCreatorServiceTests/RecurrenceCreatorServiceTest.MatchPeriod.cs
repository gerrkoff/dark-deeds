using System;
using DarkDeeds.ServiceTask.Entities.Models;
using Xunit;

namespace DarkDeeds.ServiceTask.Tests.Services.RecurrenceCreatorServiceTests
{
    public partial class RecurrenceCreatorServiceTest
    { 
        [Fact]
        public void MatchPeriod_ShouldMatchIfWithinPeriod()
        {
            var service = Service();

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
            var service = Service();

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
            var service = Service();

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
            var service = Service();

            var result = service.MatchPeriod(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4)
            }, new DateTime(2019, 12, 31));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchPeriod_ShouldNotMatchIfLessThanStartDate()
        {
            var service = Service();

            var result = service.MatchPeriod(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 4)
            }, new DateTime(2019, 9, 3));

            Assert.False(result);
        }
    }
}