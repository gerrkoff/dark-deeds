using DarkDeeds.TaskServiceApp.Infrastructure.Data;

namespace DarkDeeds.TaskServiceApp.Services.Interface
{
    public interface ISpecificationFactory
    {
        TSpec New<TSpec, TEntity>() where TSpec : ISpecification<TEntity>;
    }
}