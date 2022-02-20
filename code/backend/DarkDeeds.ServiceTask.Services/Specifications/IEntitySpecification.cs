using DarkDeeds.ServiceTask.Entities.Abstractions;
using DarkDeeds.ServiceTask.Infrastructure.Data;

namespace DarkDeeds.ServiceTask.Services.Specifications
{
    public interface IEntitySpecification<TEntity, out TSpec> : ISpecification<TEntity>
        where TSpec : class, IEntitySpecification<TEntity, TSpec>
        where TEntity: Entity 
    
    {
        TSpec FilterNotDeleted();
    }
}
