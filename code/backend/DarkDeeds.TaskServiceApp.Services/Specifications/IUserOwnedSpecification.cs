using DarkDeeds.TaskServiceApp.Entities.Models.Interfaces;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;

namespace DarkDeeds.TaskServiceApp.Services.Specifications
{
    public interface IUserOwnedSpecification<TEntity, out TSpec> : ISpecification<TEntity>
        where TSpec : class, IUserOwnedSpecification<TEntity, TSpec>
        where TEntity: IUserOwnedEntity 
    
    {
        TSpec FilterUserOwned(string userId);
    }
}
