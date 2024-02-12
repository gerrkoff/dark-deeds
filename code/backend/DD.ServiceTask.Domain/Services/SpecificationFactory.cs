using DD.ServiceTask.Domain.Specifications;

namespace DD.ServiceTask.Domain.Services;

public interface ISpecificationFactory
{
    TSpec Create<TSpec, TEntity>()
        where TSpec : ISpecification<TEntity>;
}

internal sealed class SpecificationFactory(IServiceProvider serviceProvider) : ISpecificationFactory
{
    public TSpec Create<TSpec, TEntity>()
        where TSpec : ISpecification<TEntity>
    {
        var spec = serviceProvider.GetService(typeof(TSpec)) ?? throw new InvalidOperationException($"Specification of type {typeof(TSpec).Name} not found");
        return (TSpec)spec;
    }
}
