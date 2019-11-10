using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Data.Entity.Base;
using DarkDeeds.Data.Repository;
using DarkDeeds.Models.Data;

namespace DarkDeeds.Services.Interface
{
    public interface IPermissionsService
    {
        Task CheckIfUserCanEditEntitiesAsync<T>(ICollection<IDtoWithId> dtos,
            IRepositoryNonDeletable<T> repository,
            string userId,
            string entityName)
            where T : BaseEntity, IUserOwnedEntity;
    }
}