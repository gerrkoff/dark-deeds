using System.Threading.Tasks;
using DarkDeeds.Data.Entity;
using DarkDeeds.Models;
using DarkDeeds.Services.Implementation;
using DarkDeeds.Services.Interface;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.Services.RecurrenceServiceTests
{
    public class RecurrenceServiceTest : BaseTest
    {
        [Fact]
        public async Task SaveAsync_CheckIsUserCanEdit()
        {
            var permissionMock = new Mock<IPermissionsService>();
            var repoMock = Helper.CreateRepoMock<PlannedRecurrenceEntity>();
            var service = new RecurrenceService(repoMock.Object, permissionMock.Object);

            var list = new PlannedRecurrenceDto[0];
            var userId = "userid";
            await service.SaveAsync(list, userId);
            
            permissionMock.Verify(x => x.CheckIfUserCanEditEntitiesAsync(list, repoMock.Object, userId, It.IsAny<string>()));
        }
    }
}