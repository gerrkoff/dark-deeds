using DarkDeeds.Entities.Models.Base;

namespace DarkDeeds.Tests.Services.PermissionsServiceTests.Mocks
{
    public class EntityMock : BaseEntity, IUserOwnedEntity
    {
        public string UserId { get; set; }
    }
}