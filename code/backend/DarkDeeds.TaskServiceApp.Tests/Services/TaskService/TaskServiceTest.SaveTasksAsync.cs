using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using DarkDeeds.TaskServiceApp.Infrastructure.Services;
using DarkDeeds.TaskServiceApp.Models.Dto;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DarkDeeds.TaskServiceApp.Tests.Services.TaskService
{
    public partial class TaskServiceTest : BaseTest
    {
        private Mock<IRepository<TaskEntity>> _repoMock;
        private Mock<ILogger<TaskServiceApp.Services.Implementation.TaskService>> _loggerMock;
        private TaskServiceApp.Services.Implementation.TaskService _service;
        private Mock<INotifierService> _notifierServiceMock;

        private void CreateService(params TaskEntity[] values)
        {
            _repoMock = Helper.CreateRepoMock(values);
            _loggerMock = new Mock<ILogger<TaskServiceApp.Services.Implementation.TaskService>>();
            _notifierServiceMock = new Mock<INotifierService>();
            _service = new TaskServiceApp.Services.Implementation.TaskService(
                _repoMock.Object, 
                _loggerMock.Object,
                Mapper,
                _notifierServiceMock.Object);
        }

        [Fact]
        public async Task SaveTasksAsync_ReturnTasksBack()
        {
            CreateService();

            var items = new[] {new TaskDto {Id = 1000, ClientId = -1}, new TaskDto {Id = 2000, ClientId = -2}};
            var result = (await _service.SaveTasksAsync(items, string.Empty)).ToList();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task SaveTasksAsync_StopSavingAndReturnActualTaskIfVersionMismatch()
        {
            CreateService(new TaskEntity {Uid = "uid", UserId = "userid", Version = 10, Title = "old"});

            var items = new[] {new TaskDto {Uid = "uid", Version = 9, Title = "new"}};
            var result = (await _service.SaveTasksAsync(items, "userid")).ToList();
            
            Assert.Collection(result, x =>
            {
                Assert.Equal("uid", x.Uid);
                Assert.Equal(10, x.Version);
                Assert.Equal("old", x.Title);
            });
            _repoMock.Verify(x => x.GetAll());
            _repoMock.VerifyNoOtherCalls();
        }
        
        
        [Fact]
        public async Task SaveTasksAsync_WhenDeletingCallDelete()
        {
            CreateService(new TaskEntity {Id = 1000, UserId = "1"});

            var items = new[] {new TaskDto {Id = 1000, Deleted = true}};
            await _service.SaveTasksAsync(items, "1");
            
            _repoMock.Verify(x => x.GetAll());
            _repoMock.Verify(x => x.DeleteAsync(It.Is<TaskEntity>(y => y.Id == 1000)));
            _repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveTasksAsync_WhenUpdatingUpdateAndIncrementVersionAndCallSave()
        {
            CreateService(new TaskEntity {Id = 1000, UserId = "1", Title = "Task Old", Version = 100500});

            var items = new[] {new TaskDto {Id = 1000, ClientId = 1, Title = "Task New", Version = 100500}};
            await _service.SaveTasksAsync(items, "1");
            
            _repoMock.Verify(x => x.GetAll());
            _repoMock.Verify(x => x.SaveAsync(
                It.Is<TaskEntity>(y =>
                    y.Id == 1000 &&
                    y.UserId == "1" &&
                    y.Title == "Task New" &&
                    y.Version == 100501)));
            _repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveTasksAsync_IgnoreForeignTasks()
        {
            CreateService(new TaskEntity {Uid = "uid", UserId = "foreign user"});

            var items = new[] {new TaskDto {Uid = "uid"}};
            
            var result = (await _service.SaveTasksAsync(items, "user")).ToList();

            Assert.Empty(result);
            _repoMock.Verify(x => x.GetAll());
            _repoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SaveTasksAsync_IgnoreAlreadyDeletedOnDelete()
        {
            CreateService();

            var items = new[] {new TaskDto {Uid = "uid", Deleted = true}};
            
            var result = (await _service.SaveTasksAsync(items, "user")).ToList();

            Assert.Collection(result, _ => { });
            _repoMock.Verify(x => x.GetAll());
            _repoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SaveTasksAsync_Delete()
        {
            CreateService(new TaskEntity {Id = 1000, Uid = "uid", UserId = "user"});

            var items = new[] {new TaskDto {Uid = "uid", Deleted = true, Version = 7}};
            
            var result = (await _service.SaveTasksAsync(items, "user")).ToList();

            Assert.Collection(result, x =>
            {
                Assert.Equal("uid", x.Uid);
                Assert.Equal(8, x.Version);
            });
            _repoMock.Verify(x => x.GetAll());
            _repoMock.Verify(x => x.DeleteAsync(It.Is<TaskEntity>(y => y.Id == 1000)));
            _repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveTasksAsync_Update()
        {
            CreateService(new TaskEntity {Id = 1000, Uid = "uid", UserId = "user", Title = "Task Old", Version = 100500});

            var items = new[] {new TaskDto {Uid = "uid", Title = "Task New", Version = 100500}};
            var result = (await _service.SaveTasksAsync(items, "user")).ToList();
            
            Assert.Collection(result, x =>
            {
                Assert.Equal("Task New", x.Title);
                Assert.Equal("uid", x.Uid);
                Assert.Equal(100501, x.Version);
            });
            _repoMock.Verify(x => x.GetAll());
            _repoMock.Verify(x => x.SaveAsync(
                It.Is<TaskEntity>(y =>
                    y.Id == 1000 &&
                    y.Uid == "uid" &&
                    y.UserId == "user" &&
                    y.Title == "Task New" &&
                    y.Version == 100501)));
            _repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveTasksAsync_Create()
        {
            CreateService();

            var items = new[] {new TaskDto {Uid = "uid", Title = "Task"}};
            var result = (await _service.SaveTasksAsync(items, "user")).ToList();
            
            Assert.Collection(result, x =>
            {
                Assert.Equal("Task", x.Title);
                Assert.Equal("uid", x.Uid);
                Assert.Equal(1, x.Version);
            });
            _repoMock.Verify(x => x.GetAll());
            _repoMock.Verify(x => x.SaveAsync(
                It.Is<TaskEntity>(y =>
                    y.Id == 0 &&
                    y.Uid == "uid" &&
                    y.UserId == "user" &&
                    y.Title == "Task" &&
                    y.Version == 1)));
            _repoMock.VerifyNoOtherCalls();
        }
    }
}