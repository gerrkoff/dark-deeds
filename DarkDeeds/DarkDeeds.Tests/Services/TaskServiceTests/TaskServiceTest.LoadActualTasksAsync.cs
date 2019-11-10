using System;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.Data.Entity;
using DarkDeeds.Data.Repository;
using DarkDeeds.Enums;
using DarkDeeds.Services.Implementation;
using DarkDeeds.Services.Interface;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.Services.TaskServiceTests
{
    public partial class TaskServiceTest
    {
        private IRepository<TaskEntity> DefaultRepo_LoadActualTasksAsync() => Helper.CreateRepoMock(
            new TaskEntity {UserId = "1", Id = 1, Date = new DateTime(2018, 10, 10), IsCompleted = true},
            new TaskEntity {UserId = "1", Id = 2, Date = new DateTime(2018, 10, 11)},
            new TaskEntity {UserId = "2", Id = 10},
            new TaskEntity {UserId = "1", Id = 11, Date = new DateTime(2018, 10, 19), Type = TaskTypeEnum.Additional},
            new TaskEntity {UserId = "1", Id = 3, Date = new DateTime(2018, 10, 20)},
            new TaskEntity {UserId = "1", Id = 4}
        ).Object;
        
        [Fact]
        public async Task LoadActualTasksAsync_ReturnOnlyUserTask()
        {   
            var service = new TaskService(DefaultRepo_LoadActualTasksAsync(), null, new Mock<IPermissionsService>().Object);

            var result = (await service.LoadActualTasksAsync("2", new DateTime(2000, 1, 1))).ToList();
            
            Assert.Single(result);
            Assert.Equal(10, result[0].Id);
        }
        
        [Fact]
        public async Task LoadActualTasksAsync_AdjustDateToUtc()
        {   
            var service = new TaskService(DefaultRepo_LoadActualTasksAsync(), null, new Mock<IPermissionsService>().Object);

            var result = (await service.LoadActualTasksAsync("1", new DateTime(2000, 1, 1))).ToList();
            
            Assert.Equal(DateTimeKind.Utc, result.First(x => x.Date.HasValue).Date.Value.Kind);
        }
        
        [Fact]
        public async Task LoadActualTasksAsync_IncludeNoDate()
        {
            var service = new TaskService(DefaultRepo_LoadActualTasksAsync(), null, new Mock<IPermissionsService>().Object);

            var result = (await service.LoadActualTasksAsync("1", new DateTime(2018, 10, 20))).ToList();

            Assert.Contains(result, x => x.Id == 4);
        }
        
        [Fact]
        public async Task LoadActualTasksAsync_IncludeExpiredButCompleted()
        {
            var service = new TaskService(DefaultRepo_LoadActualTasksAsync(), null, new Mock<IPermissionsService>().Object);

            var result = (await service.LoadActualTasksAsync("1", new DateTime(2018, 10, 20))).ToList();

            Assert.Contains(result, x => x.Id == 2);
        }
        
        [Fact]
        public async Task LoadActualTasksAsync_ExcludeExpiredAndCompleted()
        {
            var service = new TaskService(DefaultRepo_LoadActualTasksAsync(), null, new Mock<IPermissionsService>().Object);

            var result = (await service.LoadActualTasksAsync("1", new DateTime(2018, 10, 20))).ToList();

            Assert.DoesNotContain(result, x => x.Id == 1);
        }
        
        [Fact]
        public async Task LoadActualTasksAsync_ExcludeExpiredAdditional()
        {
            var service = new TaskService(DefaultRepo_LoadActualTasksAsync(), null, new Mock<IPermissionsService>().Object);

            var result = (await service.LoadActualTasksAsync("1", new DateTime(2018, 10, 20))).ToList();

            Assert.DoesNotContain(result, x => x.Id == 11);
        }
    }
}