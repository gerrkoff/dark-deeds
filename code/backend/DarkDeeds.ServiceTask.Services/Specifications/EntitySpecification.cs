using DarkDeeds.ServiceTask.Entities.Models.Abstractions;

namespace DarkDeeds.ServiceTask.Services.Specifications
{
    abstract class EntitySpecification<TEntity, TSpec> : Specification<TEntity>, IEntitySpecification<TEntity, TSpec>
        where TSpec : class, IEntitySpecification<TEntity, TSpec>
        where TEntity: Entity
    {
        public TSpec FilterNotDeleted()
        {
            Filters.Add(x => !x.IsDeleted);
            return this as TSpec;
        }
    }
}
