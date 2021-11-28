using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;

namespace DarkDeeds.TaskServiceApp.Services.Specifications
{
    public abstract class Specification<T> : ISpecification<T>
    {
        protected readonly List<Expression<Func<T, bool>>> Filters =
            new List<Expression<Func<T, bool>>>();

        public IQueryable<T> Apply(IQueryable<T> query)
        {
            foreach (var filter in Filters)
            {
                query = query.Where(filter);
            }

            return query;
        }
    }
}