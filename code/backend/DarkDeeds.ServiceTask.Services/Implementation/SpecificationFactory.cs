using System;
using DarkDeeds.ServiceTask.Infrastructure.Data;
using DarkDeeds.ServiceTask.Services.Interface;

namespace DarkDeeds.ServiceTask.Services.Implementation
{
    class SpecificationFactory : ISpecificationFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SpecificationFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TSpec New<TSpec, TEntity>() where TSpec : ISpecification<TEntity>
        {
            return (TSpec) _serviceProvider.GetService(typeof(TSpec));
        }
    }
}