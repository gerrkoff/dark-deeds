using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Models.Dto;
using DarkDeeds.TaskServiceApp.Models.Mapping;
using DarkDeeds.TaskServiceApp.Services.Interface;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DarkDeeds.TaskServiceApp.Tests.Services.TaskService
{
    public partial class TaskServiceTest : BaseTest
    {
        [Fact]
        public async Task SaveTasksAsync_CheckIsUserCanEdit()
        {
            var repoMock = Helper.CreateRepoMock<TaskEntity>();
            var permissionMock = new Mock<IPermissionsService>();
            var service = new TaskServiceApp.Services.Implementation.TaskService(repoMock.Object, null, permissionMock.Object, Mapper);

            var list = new TaskDto[0];
            var userId = "userid";
            await service.SaveTasksAsync(list, userId);
            
            permissionMock.Verify(x => x.CheckIfUserCanEditEntitiesAsync(list, repoMock.Object, userId, It.IsAny<string>()));
        }

        [Fact]
        public async Task SaveTasksAsync_ReturnTasksBack()
        {
            var repoMock = Helper.CreateRepoMock<TaskEntity>();
            var loggerMock = new Mock<ILogger<TaskServiceApp.Services.Implementation.TaskService>>();
            var service = new TaskServiceApp.Services.Implementation.TaskService(repoMock.Object, loggerMock.Object, new Mock<IPermissionsService>().Object, Mapper);

            var items = new[] {new TaskDto {Id = 1000, ClientId = -1}, new TaskDto {Id = 2000, ClientId = -2}};
            var result = (await service.SaveTasksAsync(items, string.Empty)).ToList();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task SaveTasksAsync_StopSavingIfThereIsNoTaskInDbWithSuchId()
        {
            var repoMock = Helper.CreateRepoMock(
                new TaskEntity {Id = 1000, UserId = "1", Version = 10, Title = "Old"});
            var loggerMock = new Mock<ILogger<TaskServiceApp.Services.Implementation.TaskService>>();
            var service = new TaskServiceApp.Services.Implementation.TaskService(repoMock.Object, loggerMock.Object, new Mock<IPermissionsService>().Object, Mapper);

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
            var loggerMock = new Mock<ILogger<TaskServiceApp.Services.Implementation.TaskService>>();
            var service = new TaskServiceApp.Services.Implementation.TaskService(repoMock.Object, loggerMock.Object, new Mock<IPermissionsService>().Object, Mapper);

            var items = new[] {new TaskDto {Id = 1000, ClientId = 1}, new TaskDto {Id = 2000, Deleted = true}};
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
            var loggerMock = new Mock<ILogger<TaskServiceApp.Services.Implementation.TaskService>>();
            var service = new TaskServiceApp.Services.Implementation.TaskService(repoMock.Object, loggerMock.Object, new Mock<IPermissionsService>().Object, Mapper);

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
            var loggerMock = new Mock<ILogger<TaskServiceApp.Services.Implementation.TaskService>>();
            var service = new TaskServiceApp.Services.Implementation.TaskService(repoMock.Object, loggerMock.Object, new Mock<IPermissionsService>().Object, Mapper);

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
            var loggerMock = new Mock<ILogger<TaskServiceApp.Services.Implementation.TaskService>>();
            var service = new TaskServiceApp.Services.Implementation.TaskService(repoMock.Object, loggerMock.Object, new Mock<IPermissionsService>().Object, Mapper);

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
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<ModelsMappingProfile>();
            });
            var mapper = config.CreateMapper();
            
            var q = mapper.ConfigurationProvider.GetMappers().ToList();
            
            var repoMock = Helper.CreateRepoMock(
                new TaskEntity {Id = 1000, UserId = "1"},
                new TaskEntity {Id = 2000, UserId = "1"});
            var loggerMock = new Mock<ILogger<TaskServiceApp.Services.Implementation.TaskService>>();
            var service = new TaskServiceApp.Services.Implementation.TaskService(repoMock.Object, loggerMock.Object, new Mock<IPermissionsService>().Object, mapper);

            var items = new[] {new TaskDto {Id = 1000, ClientId = -1}, new TaskDto {Id = 2000, ClientId = 1}};
            var result = (await service.SaveTasksAsync(items, "1")).ToList();

            Assert.Collection(result,
                x => Assert.Equal(-1, x.ClientId),
                x => Assert.Equal(1, x.ClientId));
        }
    }
}