using System;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.Data.Entity;
using DarkDeeds.Models;
using DarkDeeds.Models.Entity;
using DarkDeeds.Services.Implementation;
using Microsoft.AspNetCore.Http.Internal;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.Services
{
    public partial class TaskServiceTest
    {
        [Fact]
        public async Task SaveTasksAsync_ReturnTasksBack()
        {
            var repo = Helper.CreateRepoMock<TaskEntity>().Object;
            
            var service = new TaskService(repo);

            var items = new[] {new TaskDto {Id = 1000}, new TaskDto {Id = 2000}};
            
            var result = (await service.SaveTasksAsync(items, "1")).ToList();
            
            Assert.Equal(2, result.Count);
            Assert.Equal(1000, result[0].Id);
            Assert.Equal(2000, result[1].Id);
        }
        
        [Fact]
        public async Task SaveTasksAsync_CallDeleteWhenTaskIsDeleted()
        {
            var repoMock = Helper.CreateRepoMock<TaskEntity>();
            
            var service = new TaskService(repoMock.Object);

            var item = new TaskDto {Id = 1000, Deleted = true};
            await service.SaveTasksAsync(new[] {item}, "1");
            
            repoMock.Verify(x => x.GetAll());
            repoMock.Verify(x => x.DeleteAsync(It.Is<TaskEntity>(y => y.Id == 1000)));
            repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveTasksAsync_CallSaveWhenOrdinaryUpdatingTask()
        {
            var repoMock = Helper.CreateRepoMock<TaskEntity>();
            
            var service = new TaskService(repoMock.Object);

            var item = new TaskDto {Id = 1000};
            await service.SaveTasksAsync(new[] {item}, "1");
            
            repoMock.Verify(x => x.GetAll());
            repoMock.Verify(x => x.SaveAsync(It.Is<TaskEntity>(y => y.Id == 1000 && y.UserId == "1")));
            repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveTasksAsync_UpdateTaskWhenCreating()
        {
            var repoMock = Helper.CreateRepoMock<TaskEntity>();
            
            var service = new TaskService(repoMock.Object);

            var item = new TaskDto {Id = 1000, ClientId = -1000};
            await service.SaveTasksAsync(new[] {item}, "1");
            
            repoMock.Verify(x => x.GetAll());
            repoMock.Verify(x => x.SaveAsync(It.Is<TaskEntity>(y => y.Id == 0 && y.UserId == "1")));
            repoMock.VerifyNoOtherCalls();
        }
    }
}