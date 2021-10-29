using System;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Services.Implementation;
using DarkDeeds.TaskServiceApp.Tests.Mocks;
using Xunit;

namespace DarkDeeds.TaskServiceApp.Tests.Services.RecurrenceServiceTests
{
    public partial class RecurrenceServiceTest
    {
        [Fact]
        public async Task LoadAsync_ReturnOnlyUserEntities()
        {
            var repoMock = MocksCreator.RepoRecurrence(
                new PlannedRecurrenceEntity {UserId = "userid1"},
                new PlannedRecurrenceEntity {UserId = "userid2"}
            );
            var service = new RecurrenceService(repoMock.Object, Mapper);

            var result = await service.LoadAsync("userid2");

            Assert.Single(result);
        }
        
        [Fact]
        public async Task LoadAsync_ReturnDatesInUtc()
        {
            var repoMock = MocksCreator.RepoRecurrence(
                new PlannedRecurrenceEntity
                {
                    UserId = "userid1",
                    StartDate = new DateTime(1, DateTimeKind.Unspecified),
                    EndDate = new DateTime(1, DateTimeKind.Unspecified)
                }
            );
            var service = new RecurrenceService(repoMock.Object, Mapper);

            var result = (await service.LoadAsync("userid1")).ToList();

            Assert.Equal(DateTimeKind.Utc, result[0].StartDate.Kind);
            Assert.Equal(DateTimeKind.Utc, result[0].EndDate.Value.Kind);
        }
    }
}