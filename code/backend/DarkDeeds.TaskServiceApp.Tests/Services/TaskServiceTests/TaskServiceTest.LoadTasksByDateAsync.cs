using System;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;
using Xunit;

namespace DarkDeeds.TaskServiceApp.Tests.Services.TaskServiceTests
{
    public partial class TaskServiceTest
    {
        private TaskEntity[] Tasks() => new TaskEntity[]
        {
            new() {UserId = "1", Uid = "1", Date = new DateTime(2018, 10, 10), IsCompleted = true},
            new() {UserId = "1", Uid = "2", Date = new DateTime(2018, 10, 11)},
            new() {UserId = "2", Uid = "10", Date = new DateTime(2018, 10, 22)},
            new() {UserId = "1", Uid = "3", Date = new DateTime(2018, 10, 20)},
            new() {UserId = "1", Uid = "4"},
            new() {UserId = "1", Uid = "5", Date = new DateTime(2018, 10, 25)},
            new() {UserId = "1", Uid = "6", Date = new DateTime(2018, 10, 26)}
        };
        
        [Fact]
        public async Task LoadTasksByDateAsync_ReturnOnlyUserTask()
        {
            CreateService(Tasks());

            var result = (await _service.LoadTasksByDateAsync("2",
                new DateTime(2000, 1, 1),
                new DateTime(2020, 1, 1))).ToList();
            
            Assert.Single(result);
            Assert.Equal("10", result[0].Uid);
        }
        
        [Fact]
        public async Task LoadTasksByDateAsync_AdjustDateToUtc()
        {   
            CreateService(Tasks());

            var result = (await _service.LoadTasksByDateAsync("1",
                new DateTime(2000, 1, 1),
                new DateTime(2020, 1, 1))).ToList();
            
            Assert.Equal(DateTimeKind.Utc, result.First(x => x.Date.HasValue).Date.Value.Kind);
        }
        
        [Fact]
        public async Task LoadTasksByDateAsync_ExcludeNoDate()
        {
            CreateService(Tasks());

            var result = (await _service.LoadTasksByDateAsync("1",
                new DateTime(2018, 10, 20),
                new DateTime(2018, 10, 26))).ToList();

            Assert.DoesNotContain(result, x => x.Uid == "4");
        }
        
        [Fact]
        public async Task LoadTasksByDateAsync_IncludeOnlyTasksFromPeriod()
        {
            CreateService(Tasks());

            var result = (await _service.LoadTasksByDateAsync("1",
                new DateTime(2018, 10, 20),
                new DateTime(2018, 10, 26))).ToList();

            Assert.Contains(result, x => x.Uid == "3"); // FROM border is included
            Assert.Contains(result, x => x.Uid == "5");
            Assert.DoesNotContain(result, x => x.Uid == "1");
            Assert.DoesNotContain(result, x => x.Uid == "2");
            Assert.DoesNotContain(result, x => x.Uid == "6"); // TO border is not included
        }
    }
}