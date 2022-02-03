using DarkDeeds.ServiceTask.Infrastructure.Data;

namespace DarkDeeds.ServiceTask.Services.Interface
{
    public interface ISpecificationFactory
    {
        TSpec New<TSpec, TEntity>() where TSpec : ISpecification<TEntity>;
    }
}