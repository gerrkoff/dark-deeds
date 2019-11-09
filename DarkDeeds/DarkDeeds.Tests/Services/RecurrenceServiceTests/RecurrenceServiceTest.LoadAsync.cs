using System;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.Data.Entity;
using DarkDeeds.Services.Implementation;
using Xunit;

namespace DarkDeeds.Tests.Services.RecurrenceServiceTests
{
    public partial class RecurrenceServiceTest : BaseTest
    {
        [Fact]
        public async Task LoadAsync_ReturnOnlyUserEntities()
        {
            var repoMock = Helper.CreateRepoMock<PlannedRecurrenceEntity>(
                new PlannedRecurrenceEntity {UserId = "userid1"},
                new PlannedRecurrenceEntity {UserId = "userid2"}
            );
            var service = new RecurrenceService(repoMock.Object, null);

            var result = await service.LoadAsync("userid2");

            Assert.Equal(1, result.Count());
        }
        
        [Fact]
        public async Task LoadAsync_ReturnDatesInUtc()
        {
            var repoMock = Helper.CreateRepoMock(
                new PlannedRecurrenceEntity
                {
                    UserId = "userid1",
                    StartDate = new DateTime(1, DateTimeKind.Unspecified),
                    EndDate = new DateTime(1, DateTimeKind.Unspecified)
                }
            );
            var service = new RecurrenceService(repoMock.Object, null);

            var result = (await service.LoadAsync("userid1")).ToList();

            Assert.Equal(DateTimeKind.Utc, result[0].StartDate.Kind);
            Assert.Equal(DateTimeKind.Utc, result[0].EndDate.Value.Kind);
        }
    }
}