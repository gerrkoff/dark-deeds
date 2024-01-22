using DD.TaskService.Domain.Specifications;

namespace DD.TaskService.Domain.Services;

public interface ISpecificationFactory
{
    TSpec New<TSpec, TEntity>() where TSpec : ISpecification<TEntity>;
}

class SpecificationFactory(IServiceProvider serviceProvider) : ISpecificationFactory
{
    public TSpec New<TSpec, TEntity>() where TSpec : ISpecification<TEntity>
    {
        return (TSpec) serviceProvider.GetService(typeof(TSpec));
    }
}
