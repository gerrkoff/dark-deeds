using System;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.Data.Entity;
using DarkDeeds.Data.Repository;
using DarkDeeds.Services.Implementation;
using Xunit;

namespace DarkDeeds.Tests.Services
{
    public partial class TaskServiceTest
    {
        private IRepository<TaskEntity> DefaultRepo() => Helper.CreateRepoMock(
            new TaskEntity {UserId = "1", Id = 1, DateTime = new DateTime(2018, 10, 10), IsCompleted = true},
            new TaskEntity {UserId = "1", Id = 2, DateTime = new DateTime(2018, 10, 11)},
            new TaskEntity {UserId = "2", Id = 10},
            new TaskEntity {UserId = "1", Id = 3, DateTime = new DateTime(2018, 10, 20)},
            new TaskEntity {UserId = "1", Id = 4}
        ).Object;
        
        [Fact]
        public async Task LoadTasksAsync_ReturnOnlyUserTask()
        {   
            var service = new TaskService(DefaultRepo());

            var result = (await service.LoadTasksAsync(
                    "2",
                    null,
                    null,
                    true)
                ).ToList();
            
            Assert.Single(result);
            Assert.Equal(10, result[0].Id);
        }
        
        [Fact]
        public async Task LoadTasksAsync_AdjustDateToUtc()
        {   
            var service = new TaskService(DefaultRepo());

            var result = (await service.LoadTasksAsync(
                    "1",
                    null,
                    null,
                    true)
                ).ToList();
            
            Assert.Equal(DateTimeKind.Utc, result.First(x => x.DateTime.HasValue).DateTime.Value.Kind);
        }
        
        [Fact]
        public async Task LoadTasksAsync_ReturnWithinDatesAndNotIncludeNoDate()
        {
            var service = new TaskService(DefaultRepo());

            var result = (await service.LoadTasksAsync(
                    "1",
                    new DateTime(2018, 10, 10),
                    new DateTime(2018, 10, 20),
                    false)
                ).ToList();
            
            Assert.Collection(result, 
                x => Assert.Equal(1, x.Id),
                x => Assert.Equal(2, x.Id));
        }
        
        [Fact]
        public async Task LoadTasksAsync_ReturnWithinDatesAndIncludeNoDate()
        {
            var service = new TaskService(DefaultRepo());

            var result = (await service.LoadTasksAsync(
                    "1",
                    new DateTime(2018, 10, 10),
                    new DateTime(2018, 10, 20),
                    true)
                ).ToList();
            
            Assert.Collection(result, 
                x => Assert.Equal(1, x.Id),
                x => Assert.Equal(2, x.Id),
                x => Assert.Equal(4, x.Id));
        }
        
        [Fact]
        public async Task LoadTasksAsync_ReturnAfterFrom()
        {
            var service = new TaskService(DefaultRepo());

            var result = (await service.LoadTasksAsync(
                    "1",
                    new DateTime(2018, 10, 10),
                    null,
                    false)
                ).ToList();
            
            Assert.Collection(result, 
                x => Assert.Equal(1, x.Id),
                x => Assert.Equal(2, x.Id),
                x => Assert.Equal(3, x.Id));
        }
        
        [Fact]
        public async Task LoadTasksAsync_ReturnFromFirstUncompleted()
        {
            var service = new TaskService(DefaultRepo());

            var result = (await service.LoadTasksAsync(
                    "1",
                    null,
                    null,
                    false)
                ).ToList();
            
            Assert.Collection(result,
                x => Assert.Equal(2, x.Id),
                x => Assert.Equal(3, x.Id));
        }
        
        [Fact]
        public async Task LoadTasksAsync_ReturnFromCurrentPeriodStart()
        {
            var repo = Helper.CreateRepoMock(
                new TaskEntity {UserId = "1", Id = 1, DateTime = DateTime.UtcNow.AddDays(-1).AddMinutes(-1), IsCompleted = true},
                new TaskEntity {UserId = "1", Id = 2, DateTime = DateTime.UtcNow.AddDays(-1).AddMinutes(1), IsCompleted = true},
                new TaskEntity {UserId = "1", Id = 3, DateTime = DateTime.UtcNow.AddDays(1), IsCompleted = true}
            ).Object;
            
            var service = new TaskService(repo);

            var result = (await service.LoadTasksAsync(
                    "1",
                    null,
                    null,
                    false)
                ).ToList();
            
            Assert.Collection(result,
                x => Assert.Equal(2, x.Id),
                x => Assert.Equal(3, x.Id));
        }
    }
}