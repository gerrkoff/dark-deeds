using DarkDeeds.TaskServiceApp.Entities.Models.Abstractions;

namespace DarkDeeds.TaskServiceApp.Services.Specifications
{
    public abstract class UserOwnedSpecification<TEntity, TSpec> : Specification<TEntity>, IUserOwnedSpecification<TEntity, TSpec>
        where TSpec : class, IUserOwnedSpecification<TEntity, TSpec>
        where TEntity: IUserOwnedEntity
    {
        public TSpec FilterUserOwned(string userId)
        {
            Filters.Add(x => x.UserId == userId);
            return this as TSpec;
        }
    }
}
