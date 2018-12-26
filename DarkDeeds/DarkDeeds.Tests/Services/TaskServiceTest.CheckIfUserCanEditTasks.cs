using System.Threading.Tasks;
using DarkDeeds.Common.Exceptions;
using DarkDeeds.Data.Entity;
using DarkDeeds.Models;
using DarkDeeds.Services.Entity;
using DarkDeeds.Services.Implementation;
using Xunit;

namespace DarkDeeds.Tests.Services
{
    public partial class TaskServiceTest : BaseTest
    {
        [Fact]
        public async Task ThrowIfThereIsNotUserTask()
        {
            var repo = Helper.CreateRepoMock(
                new TaskEntity {UserId = "1", Id = 1},
                new TaskEntity {UserId = "2", Id = 2}
            ).Object;

            var service = new TaskService(repo);

            await Assert.ThrowsAsync<ServiceException>(()
                => service.CheckIfUserCanEditTasks(
                    new[] {new TaskDto {Id = 1}, new TaskDto {Id = 2}},
                    new CurrentUser {UserId = "1"}));
        }

        [Fact]
        public async Task NoExceptionIfAllTasksAreUser()
        {
            var repo = Helper.CreateRepoMock(
                new TaskEntity {UserId = "1", Id = 1},
                new TaskEntity {UserId = "2", Id = 2}
            ).Object;

            var service = new TaskService(repo);

            await service.CheckIfUserCanEditTasks(
                new[] {new TaskDto {Id = 1}},
                new CurrentUser {UserId = "1"});
        }
    }
}