using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.Common.Extensions
{
    public static class EfAsyncExtensions
    {
        public static Task<T> FirstOrDefaultSafeAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return Run(source,
                () => source.FirstOrDefaultAsync(predicate),
                () => source.FirstOrDefault(predicate));
        }
        
        public static Task<List<T>> ToListSafeAsync<T>(this IQueryable<T> source)
        {
            return Run(source,
                () => source.ToListAsync(),
                () => source.ToList());
        }
        
        public static Task<bool> AnySafeAsync<T>(this IQueryable<T> source)
        {
            return Run(source,
                () => source.AnyAsync(),
                () => source.Any());
        }
        
        public static Task<bool> AnySafeAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return Run(source,
                () => source.AnyAsync(predicate),
                () => source.Any(predicate));
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