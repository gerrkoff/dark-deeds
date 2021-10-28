using System;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Tests.Mocks;
using Xunit;

namespace DarkDeeds.TaskServiceApp.Tests.Services.RecurrenceService
{
    public partial class RecurrenceServiceTest
    {
        [Fact]
        public async Task LoadAsync_ReturnOnlyUserEntities()
        {
            var repoMock = MocksCreator.Repo(
                new PlannedRecurrenceEntity {UserId = "userid1"},
                new PlannedRecurrenceEntity {UserId = "userid2"}
            );
            var service = new TaskServiceApp.Services.Implementation.RecurrenceService(repoMock.Object, null, Mapper);

            var result = await service.LoadAsync("userid2");

            Assert.Single(result);
        }
        
        [Fact]
        public async Task LoadAsync_ReturnDatesInUtc()
        {
            var repoMock = MocksCreator.Repo(
                new PlannedRecurrenceEntity
                {
                    UserId = "userid1",
                    StartDate = new DateTime(1, DateTimeKind.Unspecified),
                    EndDate = new DateTime(1, DateTimeKind.Unspecified)
                }
            );
            var service = new TaskServiceApp.Services.Implementation.RecurrenceService(repoMock.Object, null, Mapper);

            var result = (await service.LoadAsync("userid1")).ToList();

            Assert.Equal(DateTimeKind.Utc, result[0].StartDate.Kind);
            Assert.Equal(DateTimeKind.Utc, result[0].EndDate.Value.Kind);
        }
    }
}