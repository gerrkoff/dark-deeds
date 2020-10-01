﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Entities.Models.Base;
using DarkDeeds.Infrastructure.Interfaces.Data;
using DarkDeeds.Services.Dto.Base;

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