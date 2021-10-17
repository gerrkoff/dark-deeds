using System;
using DarkDeeds.TaskServiceApp.Entities.Enums;
using DarkDeeds.TaskServiceApp.Entities.Models;
using Xunit;

namespace DarkDeeds.TaskServiceApp.Tests.Services.RecurrenceCreatorService
{
    public partial class RecurrenceCreatorServiceTest
    {
        [Fact]
        public void MatchWeekday_ShouldMatchOneWeekday()
        {
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(null, null, null, null, null, null, null, Mapper);

            var result = service.MatchWeekday(new PlannedRecurrenceEntity
            {
                EveryWeekday = RecurrenceWeekdayEnum.Monday
            }, new DateTime(2019, 9, 2));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchWeekday_ShouldMatchOneOfTwoWeekdays()
        {
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(null, null, null, null, null, null, null, Mapper);

            var result = service.MatchWeekday(new PlannedRecurrenceEntity
            {
                EveryWeekday = RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Friday
            }, new DateTime(2019, 9, 6));

            Assert.True(result);
        }
        
        [Fact]
        public void MatchWeekday_ShouldNotMatchOneOfWeekdays()
        {
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(null, null, null, null, null, null, null, Mapper);

            var result = service.MatchWeekday(new PlannedRecurrenceEntity
            {
                EveryWeekday = RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Friday
            }, new DateTime(2019, 9, 5));

            Assert.False(result);
        }
        
        [Fact]
        public void MatchWeekday_ShouldMatchIfWeekdayIsNull()
        {
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(null, null, null, null, null, null, null, Mapper);

            var result = service.MatchWeekday(new PlannedRecurrenceEntity
            {
                EveryWeekday = null
            }, new DateTime());

            Assert.True(result);
        }
    }
}