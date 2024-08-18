using DD.Shared.Details.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace DD.Shared.Details.Common;

public class CacheProvider(IMemoryCache cache) : ICacheProvider
{
    public T? GetValue<T>(object key)
    {
        return cache.Get<T>(key);
    }

    public void SetValue<T>(object key, T value)
    {
        cache.Set(key, value, TimeSpan.FromHours(24));
    }

    public void Evict(object key)
    {
        cache.Remove(key);
    }
}
