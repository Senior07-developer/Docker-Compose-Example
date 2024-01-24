using System.Text.Json;
using Docker.Compose.API.Cache;
using Microsoft.Extensions.Caching.Distributed;

namespace Docker.Compose.API.Extensions;

public static class CacheExtensions
{
    public static async Task<TItem?> GetOrCreateAsync<TItem>(this IDistributedCache cache, string key, Func<Task<TItem>> getMethod,
        DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
    {
        var value = await cache.GetStringAsync(key, cancellationToken);
        if (!string.IsNullOrEmpty(value))
        {
            return JsonSerializer.Deserialize<TItem>(value);
        }

        var item = await getMethod();
        if (item is null)
        {
            return item;
        }

        options ??= CacheOptions.DefaultExpiration;
        
        await cache.SetStringAsync(key, JsonSerializer.Serialize(item), options, cancellationToken);

        return item;
    }
}