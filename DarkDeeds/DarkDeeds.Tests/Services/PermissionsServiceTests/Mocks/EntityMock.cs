using DarkDeeds.Data.Entity.Base;

namespace DarkDeeds.Tests.Services.PermissionsServiceTests.Mocks
{
    public class EntityMock : BaseEntity, IUserOwnedEntity
    {
        public string UserId { get; set; }
    }
}