using DD.ServiceTask.Domain.Specifications;

namespace DD.ServiceTask.Domain.Services;

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
