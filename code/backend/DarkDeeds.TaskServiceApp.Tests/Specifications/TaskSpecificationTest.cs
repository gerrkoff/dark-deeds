using System;
using System.Collections.Generic;
using System.Linq;
using DarkDeeds.TaskServiceApp.Entities.Enums;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Services.Specifications;
using Xunit;

namespace DarkDeeds.TaskServiceApp.Tests.Specifications
{
    public class TaskSpecificationTest
    {
        private List<TaskEntity> Collection() => new()
        {
            new() {Uid = "1", Date = new DateTime(2018, 10, 10), IsCompleted = true},
            new() {Uid = "2", Date = new DateTime(2018, 10, 11)},
            new() {Uid = "11", Date = new DateTime(2018, 10, 19), Type = TaskTypeEnum.Additional},
            new() {Uid = "3", Date = new DateTime(2018, 10, 20)},
            new() {Uid = "4"}
        };

        [Fact]
        public void FilterActual_IncludeNoDate()
        {
            var service = new TaskSpecification().FilterActual(new DateTime(2018, 10, 20));

            var result = service.Apply(Collection().AsQueryable()).ToList();

            Assert.Contains(result, x => x.Uid == "4");
        }
        
        [Fact]
        public void FilterActual_IncludeExpiredButCompleted()
        {
            var service = new TaskSpecification().FilterActual(new DateTime(2018, 10, 20));

            var result = service.Apply(Collection().AsQueryable()).ToList();
        
            Assert.Contains(result, x => x.Uid == "2");
        }
        
        [Fact]
        public void FilterActual_ExcludeExpiredAndCompleted()
        {
            var service = new TaskSpecification().FilterActual(new DateTime(2018, 10, 20));

            var result = service.Apply(Collection().AsQueryable()).ToList();
        
            Assert.DoesNotContain(result, x => x.Uid == "1");
        }
        
        [Fact]
        public void FilterActual_ExcludeExpiredAdditional()
        {
            var service = new TaskSpecification().FilterActual(new DateTime(2018, 10, 20));

            var result = service.Apply(Collection().AsQueryable()).ToList();
        
            Assert.DoesNotContain(result, x => x.Uid == "11");
        }
    }
}