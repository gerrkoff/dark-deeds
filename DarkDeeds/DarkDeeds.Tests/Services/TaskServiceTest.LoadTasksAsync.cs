using System;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.Data.Entity;
using DarkDeeds.Models.Entity;
using DarkDeeds.Services.Implementation;
using Xunit;

namespace DarkDeeds.Tests.Services
{
    public partial class TaskServiceTest
    {
        [Fact]
        public async Task LoadTasksAsync_ReturnOnlyUserTask()
        {
            var repo = Helper.CreateRepoMock(
                new TaskEntity {UserId = "1", Id = 1000},
                new TaskEntity {UserId = "2", Id = 2000}
            ).Object;
            
            var service = new TaskService(repo);

            var result = (await service.LoadTasksAsync("1")).ToList();
            Assert.Single(result);
            Assert.Equal(1000, result[0].Id);
        }
        
        [Fact]
        public async Task LoadTasksAsync_AdjustDateToUtc()
        {
            var repo = Helper.CreateRepoMock(
                new TaskEntity {UserId = "1", DateTime = new DateTime()}
            ).Object;
            
            var service = new TaskService(repo);

            var result = (await service.LoadTasksAsync("1")).ToList();
            Assert.Equal(DateTimeKind.Utc, result[0].DateTime.Value.Kind);
        }
    }
}