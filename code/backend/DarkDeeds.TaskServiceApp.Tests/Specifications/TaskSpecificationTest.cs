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
            new() {Id = 1, Date = new DateTime(2018, 10, 10), IsCompleted = true},
            new() {Id = 2, Date = new DateTime(2018, 10, 11)},
            new() {Id = 11, Date = new DateTime(2018, 10, 19), Type = TaskTypeEnum.Additional},
            new() {Id = 3, Date = new DateTime(2018, 10, 20)},
            new() {Id = 4}
        };

        [Fact]
        public void LoadActualTasksAsync_IncludeNoDate()
        {
            var service = new TaskSpecification().FilterActual(new DateTime(2018, 10, 20));

            var result = service.Apply(Collection().AsQueryable()).ToList();

            Assert.Contains(result, x => x.Id == 4);
        }
        
        [Fact]
        public void LoadActualTasksAsync_IncludeExpiredButCompleted()
        {
            var service = new TaskSpecification().FilterActual(new DateTime(2018, 10, 20));

            var result = service.Apply(Collection().AsQueryable()).ToList();
        
            Assert.Contains(result, x => x.Id == 2);
        }
        
        [Fact]
        public void LoadActualTasksAsync_ExcludeExpiredAndCompleted()
        {
            var service = new TaskSpecification().FilterActual(new DateTime(2018, 10, 20));

            var result = service.Apply(Collection().AsQueryable()).ToList();
        
            Assert.DoesNotContain(result, x => x.Id == 1);
        }
        
        [Fact]
        public void LoadActualTasksAsync_ExcludeExpiredAdditional()
        {
            var service = new TaskSpecification().FilterActual(new DateTime(2018, 10, 20));

            var result = service.Apply(Collection().AsQueryable()).ToList();
        
            Assert.DoesNotContain(result, x => x.Id == 11);
        }
    }
}