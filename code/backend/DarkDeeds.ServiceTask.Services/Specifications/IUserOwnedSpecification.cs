using DarkDeeds.ServiceTask.Entities.Abstractions;
using DarkDeeds.ServiceTask.Infrastructure.Data;

namespace DarkDeeds.ServiceTask.Services.Specifications
{
    public interface IUserOwnedSpecification<TEntity, out TSpec> : ISpecification<TEntity>
        where TSpec : class, IUserOwnedSpecification<TEntity, TSpec>
        where TEntity: IUserOwnedEntity 
    
    {
        TSpec FilterUserOwned(string userId);
        TSpec FilterForeignUserOwned(string userId, string[] uidList);
    }
}
