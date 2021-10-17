using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models.Base;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using DarkDeeds.TaskServiceApp.Models.Dto.Base;

namespace DarkDeeds.TaskServiceApp.Services.Interface
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