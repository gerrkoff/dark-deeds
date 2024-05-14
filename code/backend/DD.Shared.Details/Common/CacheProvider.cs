using DD.Shared.Details.Abstractions;
using DD.Shared.Details.Abstractions.Models;
using Microsoft.Extensions.Caching.Memory;

namespace DD.Shared.Details.Common;

public class CacheProvider(IMemoryCache cache) : ICacheProvider
{
    public T? GetValue<T>(ICacheKey key)
    {
        return cache.Get<T>(key);
    }

    public void SetValue<T>(ICacheKey key, T value)
    {
        cache.Set(key, value, TimeSpan.FromMinutes(60));
    }

    public void Evict(ICacheKey key)
    {
        cache.Remove(key);
    }
}
