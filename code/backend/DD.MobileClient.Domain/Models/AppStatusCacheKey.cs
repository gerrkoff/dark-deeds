using DD.Shared.Details.Abstractions.Models;

namespace DD.MobileClient.Domain.Models;

public record AppStatusCacheKey(string MobileKey) : ICacheKey;
