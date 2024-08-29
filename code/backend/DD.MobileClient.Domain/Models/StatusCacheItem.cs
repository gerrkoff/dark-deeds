namespace DD.MobileClient.Domain.Models;

public record StatusCacheItem<T>(DateTime Date, T Item);
