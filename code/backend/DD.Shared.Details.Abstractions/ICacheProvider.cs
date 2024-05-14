namespace DD.Shared.Details.Abstractions;

public interface ICacheProvider
{
    T? GetValue<T>(object key);

    void SetValue<T>(object key, T value);

    void Evict(object key);
}
