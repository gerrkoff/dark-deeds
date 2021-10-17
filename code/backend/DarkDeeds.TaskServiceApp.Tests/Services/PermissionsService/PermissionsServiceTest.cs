using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Models.Exceptions;
using DarkDeeds.TaskServiceApp.Tests.Services.PermissionsService.Mocks;
using Xunit;

namespace DarkDeeds.TaskServiceApp.Tests.Services.PermissionsService
{
    public class PermissionsServiceTest : BaseTest
    {        
        [Fact]
        public async Task CheckIfUserCanEditEntitiesAsync_ThrowsNoExceptionIfDtosAreValid()
        {
            var dtos = new[] {new DtoMock {Id = 1}, new DtoMock {Id = 2}};
            var repo = Helper.CreateRepoNonDeletableMock(
                new EntityMock {Id = 1, UserId = "userid1"},
                new EntityMock {Id = 2, UserId = "userid1"});
            
            var service = new TaskServiceApp.Services.Implementation.PermissionsService();

            await service.CheckIfUserCanEditEntitiesAsync(dtos, repo.Object, "userid1", "");
        }
        
        [Fact]
        public async Task CheckIfUserCanEditEntitiesAsync_ThrowsExceptionIfDtosAreNotValid()
        {
            var dtos = new[] {new DtoMock {Id = 1}, new DtoMock {Id = 2}};
            var repo = Helper.CreateRepoNonDeletableMock(
                new EntityMock {Id = 1, UserId = "userid1"},
                new EntityMock {Id = 2, UserId = "userid2"});
            
            var service = new TaskServiceApp.Services.Implementation.PermissionsService();

            await Assert.ThrowsAsync<ServiceException>(() =>
                service.CheckIfUserCanEditEntitiesAsync(dtos, repo.Object, "userid1", ""));
        }
    }
}