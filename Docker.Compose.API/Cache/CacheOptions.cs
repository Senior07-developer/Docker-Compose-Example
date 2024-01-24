using Microsoft.Extensions.Caching.Distributed;

namespace Docker.Compose.API.Cache;

public static class CacheOptions
{
    public static DistributedCacheEntryOptions DefaultExpiration =>
        new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30) };
}