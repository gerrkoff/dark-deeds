using DD.Shared.Details.Abstractions.Models;

namespace DD.MobileClient.Domain.Models;

public record WidgetStatusCacheKey(string MobileKey) : ICacheKey;
