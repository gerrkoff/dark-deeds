using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.TaskServiceApp.EfCoreExtensions
{
    public static class EfAsyncExtensions
    {   
        public static Task<List<T>> ToListSafeAsync<T>(this IQueryable<T> source)
        {
            return Run(source,
                () => source.ToListAsync(),
                () => source.ToList());
        }

        private static Task<TResult> Run<TSource, TResult>(
            IQueryable<TSource> source,
            Func<Task<TResult>> operationAsync,
            Func<TResult> operationSync)
        {
            return source is IAsyncEnumerable<TSource>
                ? operationAsync()
                : Task.FromResult(operationSync());
        }
    }
}