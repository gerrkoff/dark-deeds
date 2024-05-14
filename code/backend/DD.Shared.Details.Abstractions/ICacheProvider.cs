using DD.Shared.Details.Abstractions.Models;

namespace DD.Shared.Details.Abstractions;

public interface ICacheProvider
{
    T? GetValue<T>(ICacheKey key);

    void SetValue<T>(ICacheKey key, T value);

    void Evict(ICacheKey key);
}
