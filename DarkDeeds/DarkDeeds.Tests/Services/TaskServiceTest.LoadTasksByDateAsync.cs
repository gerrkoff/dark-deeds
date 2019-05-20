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
        private IRepository<TaskEntity> DefaultRepo_LoadTasksByDateAsync() => Helper.CreateRepoMock(
            new TaskEntity {UserId = "1", Id = 1, DateTime = new DateTime(2018, 10, 10), IsCompleted = true},
            new TaskEntity {UserId = "1", Id = 2, DateTime = new DateTime(2018, 10, 11)},
            new TaskEntity {UserId = "2", Id = 10, DateTime = new DateTime(2018, 10, 22)},
            new TaskEntity {UserId = "1", Id = 3, DateTime = new DateTime(2018, 10, 20)},
            new TaskEntity {UserId = "1", Id = 4},
            new TaskEntity {UserId = "1", Id = 5, DateTime = new DateTime(2018, 10, 25)},
            new TaskEntity {UserId = "1", Id = 6, DateTime = new DateTime(2018, 10, 26)}
        ).Object;
        
        [Fact]
        public async Task LoadTasksByDateAsync_ReturnOnlyUserTask()
        {   
            var service = new TaskService(DefaultRepo_LoadTasksByDateAsync());

            var result = (await service.LoadTasksByDateAsync("2",
                new DateTime(2000, 1, 1),
                new DateTime(2020, 1, 1))).ToList();
            
            Assert.Single(result);
            Assert.Equal(10, result[0].Id);
        }
        
        [Fact]
        public async Task LoadTasksByDateAsync_AdjustDateToUtc()
        {   
            var service = new TaskService(DefaultRepo_LoadTasksByDateAsync());

            var result = (await service.LoadTasksByDateAsync("1",
                new DateTime(2000, 1, 1),
                new DateTime(2020, 1, 1))).ToList();
            
            Assert.Equal(DateTimeKind.Utc, result.First(x => x.DateTime.HasValue).DateTime.Value.Kind);
        }
        
        [Fact]
        public async Task LoadActualTasksAsync_ExcludeNoDate()
        {
            var service = new TaskService(DefaultRepo_LoadTasksByDateAsync());

            var result = (await service.LoadTasksByDateAsync("1",
                new DateTime(2018, 10, 20),
                new DateTime(2018, 10, 26))).ToList();

            Assert.DoesNotContain(result, x => x.Id == 4);
        }
        
        [Fact]
        public async Task LoadActualTasksAsync_IncludeOnlyTasksFromPeriod()
        {
            var service = new TaskService(DefaultRepo_LoadTasksByDateAsync());

            var result = (await service.LoadTasksByDateAsync("1",
                new DateTime(2018, 10, 20),
                new DateTime(2018, 10, 26))).ToList();

            Assert.Contains(result, x => x.Id == 3); // FROM border is included
            Assert.Contains(result, x => x.Id == 5);
            Assert.DoesNotContain(result, x => x.Id == 1);
            Assert.DoesNotContain(result, x => x.Id == 2);
            Assert.DoesNotContain(result, x => x.Id == 6); // TO border is not included
        }
    }
}