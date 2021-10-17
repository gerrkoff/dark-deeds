using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.EfCoreExtensions;
using DarkDeeds.TaskServiceApp.Entities.Models.Base;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using DarkDeeds.TaskServiceApp.Models.Dto.Base;
using DarkDeeds.TaskServiceApp.Models.Exceptions;
using DarkDeeds.TaskServiceApp.Services.Interface;

namespace DarkDeeds.TaskServiceApp.Services.Implementation
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