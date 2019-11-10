using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.Data.Entity;
using DarkDeeds.Models;
using DarkDeeds.Services.Implementation;
using DarkDeeds.Services.Interface;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.Services.TaskServiceTests
{
    public partial class TaskServiceTest : BaseTest
    {
        [Fact]
        public async Task SaveTasksAsync_ReturnTasksBack()
        {
            var repoMock = Helper.CreateRepoMock<TaskEntity>();
            var loggerMock = new Mock<ILogger<TaskService>>();
            var service = new TaskService(repoMock.Object, loggerMock.Object, new Mock<IPermissionsService>().Object);

            var items = new[] {new TaskDto {Id = 1000, ClientId = -1}, new TaskDto {Id = 2000, ClientId = -2}};
            var result = (await service.SaveTasksAsync(items, string.Empty)).ToList();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task SaveTasksAsync_StopSavingIfThereIsNoTaskInDbWithSuchId()
        {
            var repoMock = Helper.CreateRepoMock(
                new TaskEntity {Id = 1000, UserId = "1", Version = 10, Title = "Old"});
            var loggerMock = new Mock<ILogger<TaskService>>();
            var service = new TaskService(repoMock.Object, loggerMock.Object, new Mock<IPermissionsService>().Object);

            var items = new[] {new TaskDto {Id = 1000, ClientId = 20, Version = 5, Title = "New"}};
            var result = (await service.SaveTasksAsync(items, "1")).ToList();
            
            repoMock.Verify(x => x.GetAll());
            repoMock.VerifyNoOtherCalls();
            Assert.Single(result);
            Assert.Equal(1000, result[0].Id);
            Assert.Equal(10, result[0].Version);
            Assert.Equal("Old", result[0].Title);
        }
        
        [Fact]
        public async Task SaveTasksAsync_StopSavingAndReturnActualTaskIfVersionMismatch()
        {
            var repoMock = Helper.CreateRepoMock<TaskEntity>();
            var loggerMock = new Mock<ILogger<TaskService>>();
            var service = new TaskService(repoMock.Object, loggerMock.Object, new Mock<IPermissionsService>().Object);

            var items = new[] {new TaskDto {Id = 1000, ClientId = 0}, new TaskDto {Id = 2000, Deleted = true}};
            var result = (await service.SaveTasksAsync(items, string.Empty)).ToList();
            
            repoMock.Verify(x => x.GetAll());
            repoMock.VerifyNoOtherCalls();
            Assert.Empty(result);
        }
        
        
        [Fact]
        public async Task SaveTasksAsync_WhenDeletingCallDelete()
        {
            var repoMock = Helper.CreateRepoMock(
                new TaskEntity {Id = 1000, UserId = "1"});
            var loggerMock = new Mock<ILogger<TaskService>>();
            var service = new TaskService(repoMock.Object, loggerMock.Object, new Mock<IPermissionsService>().Object);

            var items = new[] {new TaskDto {Id = 1000, Deleted = true}};
            await service.SaveTasksAsync(items, "1");
            
            repoMock.Verify(x => x.GetAll());
            repoMock.Verify(x => x.DeleteAsync(It.Is<TaskEntity>(y => y.Id == 1000)));
            repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveTasksAsync_WhenCreatingResetIdAndSetUserIdAndCallSave()
        {
            var repoMock = Helper.CreateRepoMock<TaskEntity>();
            var loggerMock = new Mock<ILogger<TaskService>>();
            var service = new TaskService(repoMock.Object, loggerMock.Object, new Mock<IPermissionsService>().Object);

            var items = new[] {new TaskDto {Id = 1000, ClientId = -1, Title = "Task"}};
            await service.SaveTasksAsync(items, "1");
            
            repoMock.Verify(x => x.GetAll());
            repoMock.Verify(x => x.SaveAsync(
                It.Is<TaskEntity>(y =>
                    y.Id == 0 &&
                    y.UserId == "1" &&
                    y.Title == "Task" &&
                    y.Version == 0)));
            repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveTasksAsync_WhenUpdatingUpdateAndIncrementVersionAndCallSave()
        {
            var repoMock = Helper.CreateRepoMock(
                new TaskEntity {Id = 1000, UserId = "1", Title = "Task Old", Version = 100500});
            var loggerMock = new Mock<ILogger<TaskService>>();
            var service = new TaskService(repoMock.Object, loggerMock.Object, new Mock<IPermissionsService>().Object);

            var items = new[] {new TaskDto {Id = 1000, ClientId = 1, Title = "Task New", Version = 100500}};
            await service.SaveTasksAsync(items, "1");
            
            repoMock.Verify(x => x.GetAll());
            repoMock.Verify(x => x.SaveAsync(
                It.Is<TaskEntity>(y =>
                    y.Id == 1000 &&
                    y.UserId == "1" &&
                    y.Title == "Task New" &&
                    y.Version == 100501)));
            repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveTasksAsync_KeepClientIdWhenCreatingOrUpdating()
        {
            var repoMock = Helper.CreateRepoMock(
                new TaskEntity {Id = 1000, UserId = "1"},
                new TaskEntity {Id = 2000, UserId = "1"});
            var loggerMock = new Mock<ILogger<TaskService>>();
            var service = new TaskService(repoMock.Object, loggerMock.Object, new Mock<IPermissionsService>().Object);

            var items = new[] {new TaskDto {Id = 1000, ClientId = -1}, new TaskDto {Id = 2000, ClientId = 1}};
            var result = (await service.SaveTasksAsync(items, "1")).ToList();

            Assert.Collection(result,
                x => Assert.Equal(-1, x.ClientId),
                x => Assert.Equal(1, x.ClientId));
        }
    }
}