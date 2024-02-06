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
        var spec = serviceProvider.GetService(typeof(TSpec));

        if (spec is null)
            throw new InvalidOperationException($"Specification of type {typeof(TSpec).Name} not found");

        return (TSpec) spec;
    }
}
