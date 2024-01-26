using DD.ServiceTask.Domain.Entities.Abstractions;

namespace DD.ServiceTask.Domain.Specifications;

public interface IUserOwnedSpecification<TEntity, out TSpec> : ISpecification<TEntity>
    where TSpec : class, IUserOwnedSpecification<TEntity, TSpec>
    where TEntity: IUserOwnedEntity

{
    TSpec FilterUserOwned(string userId);
    TSpec FilterForeignUserOwned(string userId, string[] uidList);
}

public abstract class UserOwnedSpecification<TEntity, TSpec> : EntitySpecification<TEntity, TSpec>, IUserOwnedSpecification<TEntity, TSpec>
    where TSpec : class, IEntitySpecification<TEntity, TSpec>, IUserOwnedSpecification<TEntity, TSpec>
    where TEntity: Entity, IUserOwnedEntity
{
    public TSpec FilterUserOwned(string userId)
    {
        Filters.Add(x => x.UserId == userId);
        return this as TSpec;
    }

    public TSpec FilterForeignUserOwned(string userId, string[] uidList)
    {
        Filters.Add(x => uidList.Contains(x.Uid) && x.UserId != userId);
        return this as TSpec;
    }
}
