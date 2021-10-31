using System;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;
using Xunit;

namespace DarkDeeds.TaskServiceApp.Tests.Services.TaskServiceTests
{
    public partial class TaskServiceTest
    {
        [Fact]
        public async Task LoadTasksByDateAsync_Positive()
        {
            var userId = "userid";
            var from = new DateTime(2018, 10, 20);
            var to = new DateTime(2018, 10, 26);
            
            CreateService(new TaskEntity {Date = new DateTime(2018, 10, 10)});

            var result = (await _service.LoadTasksByDateAsync(userId, from, to)).ToList();

            Assert.Equal(DateTimeKind.Utc, result.First(x => x.Date.HasValue)!.Date!.Value.Kind);
            _taskSpecMock.Verify(x => x.FilterUserOwned(userId));
            _taskSpecMock.Verify(x => x.FilterDateInterval(from, to));
            _repoMock.Verify(x => x.GetBySpecAsync(_taskSpecMock.Object));
        }
    }
}