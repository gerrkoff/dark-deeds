using DarkDeeds.TaskServiceApp.Entities.Models.Abstractions;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;

namespace DarkDeeds.TaskServiceApp.Services.Specifications
{
    public interface IEntitySpecification<TEntity, out TSpec> : ISpecification<TEntity>
        where TSpec : class, IEntitySpecification<TEntity, TSpec>
        where TEntity: Entity 
    
    {
        TSpec FilterNotDeleted();
    }
}
