using DarkDeeds.TaskServiceApp.Entities.Models.Base;

namespace DarkDeeds.TaskServiceApp.Tests.Services.PermissionsService.Mocks
{
    public class EntityMock : BaseEntity, IUserOwnedEntity
    {
        public string UserId { get; set; }
    }
}