using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.Common.Extensions
{
    public static class EfExtensions
    {
        public static Task<List<TSource>> ToListSafeAsync<TSource>(this IQueryable<TSource> source)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                
                case IAsyncEnumerable<TSource> _:
                    return source.ToListAsync();
                
                default:
                    return Task.FromResult(source.ToList());
            }
        }
    }
}