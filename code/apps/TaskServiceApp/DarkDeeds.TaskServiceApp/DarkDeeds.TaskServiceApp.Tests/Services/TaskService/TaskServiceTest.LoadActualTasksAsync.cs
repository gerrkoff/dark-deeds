using System;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Enums;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using DarkDeeds.TaskServiceApp.Services.Interface;
using Moq;
using Xunit;

namespace DarkDeeds.TaskServiceApp.Tests.Services.TaskService
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
            var service = new TaskServiceApp.Services.Implementation.TaskService(DefaultRepo_LoadActualTasksAsync(), null, new Mock<IPermissionsService>().Object, Mapper);

            var result = (await service.LoadActualTasksAsync("2", new DateTime(2000, 1, 1))).ToList();
            
            Assert.Single(result);
            Assert.Equal(10, result[0].Id);
        }
        
        [Fact]
        public async Task LoadActualTasksAsync_AdjustDateToUtc()
        {   
            var service = new TaskServiceApp.Services.Implementation.TaskService(DefaultRepo_LoadActualTasksAsync(), null, new Mock<IPermissionsService>().Object, Mapper);

            var result = (await service.LoadActualTasksAsync("1", new DateTime(2000, 1, 1))).ToList();
            
            Assert.Equal(DateTimeKind.Utc, result.First(x => x.Date.HasValue).Date.Value.Kind);
        }
        
        [Fact]
        public async Task LoadActualTasksAsync_IncludeNoDate()
        {
            var service = new TaskServiceApp.Services.Implementation.TaskService(DefaultRepo_LoadActualTasksAsync(), null, new Mock<IPermissionsService>().Object, Mapper);

            var result = (await service.LoadActualTasksAsync("1", new DateTime(2018, 10, 20))).ToList();

            Assert.Contains(result, x => x.Id == 4);
        }
        
        [Fact]
        public async Task LoadActualTasksAsync_IncludeExpiredButCompleted()
        {
            var service = new TaskServiceApp.Services.Implementation.TaskService(DefaultRepo_LoadActualTasksAsync(), null, new Mock<IPermissionsService>().Object, Mapper);

            var result = (await service.LoadActualTasksAsync("1", new DateTime(2018, 10, 20))).ToList();

            Assert.Contains(result, x => x.Id == 2);
        }
        
        [Fact]
        public async Task LoadActualTasksAsync_ExcludeExpiredAndCompleted()
        {
            var service = new TaskServiceApp.Services.Implementation.TaskService(DefaultRepo_LoadActualTasksAsync(), null, new Mock<IPermissionsService>().Object, Mapper);

            var result = (await service.LoadActualTasksAsync("1", new DateTime(2018, 10, 20))).ToList();

            Assert.DoesNotContain(result, x => x.Id == 1);
        }
        
        [Fact]
        public async Task LoadActualTasksAsync_ExcludeExpiredAdditional()
        {
            var service = new TaskServiceApp.Services.Implementation.TaskService(DefaultRepo_LoadActualTasksAsync(), null, new Mock<IPermissionsService>().Object, Mapper);

            var result = (await service.LoadActualTasksAsync("1", new DateTime(2018, 10, 20))).ToList();

            Assert.DoesNotContain(result, x => x.Id == 11);
        }
    }
}