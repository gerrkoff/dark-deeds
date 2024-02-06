using DD.ServiceTask.Domain.Entities.Abstractions;

namespace DD.ServiceTask.Domain.Specifications;

public interface IEntitySpecification<TEntity, out TSpec> : ISpecification<TEntity>
    where TSpec : class, IEntitySpecification<TEntity, TSpec>
    where TEntity: Entity

{
    TSpec FilterNotDeleted();
}

public abstract class EntitySpecification<TEntity, TSpec> : Specification<TEntity>, IEntitySpecification<TEntity, TSpec>
    where TSpec : class, IEntitySpecification<TEntity, TSpec>
    where TEntity: Entity
{
    public TSpec FilterNotDeleted()
    {
        Filters.Add(x => !x.IsDeleted);
        return (this as TSpec)!;
    }
}
