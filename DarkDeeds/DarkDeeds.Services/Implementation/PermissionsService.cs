using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.Common.Exceptions;
using DarkDeeds.Common.Extensions;
using DarkDeeds.Entities.Models.Base;
using DarkDeeds.Infrastructure.Interfaces.Data;
using DarkDeeds.Models.Data;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.Services.Implementation
{
    public class PermissionsService : IPermissionsService
    {
        public async Task CheckIfUserCanEditEntitiesAsync<T>(ICollection<IDtoWithId> dtos,
            IRepositoryNonDeletable<T> repository,
            string userId,
            string entityName)
            where T : BaseEntity, IUserOwnedEntity
        {
            int[] ids = dtos.Select(x => x.Id).ToArray();

            bool notUserEntities = await repository.GetAll().AnySafeAsync(x =>
                !string.Equals(x.UserId, userId) &&
                ids.Contains(x.Id));

            if (notUserEntities)
                throw ServiceException.InvalidEntity(entityName);
        }
    }
}