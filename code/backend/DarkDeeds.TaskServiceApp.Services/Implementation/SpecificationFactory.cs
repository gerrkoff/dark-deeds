using System;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using DarkDeeds.TaskServiceApp.Services.Interface;

namespace DarkDeeds.TaskServiceApp.Services.Implementation
{
    public class SpecificationFactory : ISpecificationFactory
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