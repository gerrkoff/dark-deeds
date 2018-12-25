using System.Linq;
using DarkDeeds.Common.Exceptions;
using DarkDeeds.Data.Entity;
using DarkDeeds.Data.Entity.Base;
using DarkDeeds.Data.Repository;
using DarkDeeds.Models;
using DarkDeeds.Services.Entity;
using DarkDeeds.Services.Implementation;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.Services
{
    public partial class TaskServiceTest
    {
        [Fact]
        public void ThrowIfThereIsNotUserTask()
        {
            var repo = Helper.CreateRepoMock(new TaskEntity {UserId = "1", Id = 1}, new TaskEntity {UserId = "2", Id = 2})
                .Object;

            var service = new TaskService(repo);

            Assert.Throws<ServiceException>(()
                => service.CheckIfUserCanEditTasks(
                    new[] {new TaskDto {Id = 1}, new TaskDto {Id = 2}},
                    new CurrentUser {UserId = "1"}));
        }
        
        [Fact]
        public void NoExceptionIfAllTasksAreUser()
        {
            var repo = Helper.CreateRepoMock(new TaskEntity {UserId = "1", Id = 1}, new TaskEntity {UserId = "2", Id = 2})
                .Object;

            var service = new TaskService(repo);

            service.CheckIfUserCanEditTasks(
                new[] {new TaskDto {Id = 1}},
                new CurrentUser {UserId = "1"});
        }
    }
}