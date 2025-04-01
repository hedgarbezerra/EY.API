using EY.Domain.Contracts;
using EY.Shared.Attributes;
using Microsoft.Extensions.Caching.Distributed;

namespace EY.Infrastructure.Cache;

[BindInterface(typeof(IRedisCache))]
public class RedisCache : IRedisCache
{
    private readonly IDistributedCache _distributedCache;
    private readonly DistributedCacheEntryOptions _distributedCacheEntryOptions;
    private readonly IJsonHandler _jsonHandler;

    public RedisCache(IDistributedCache distributedCache, IJsonHandler jsonHandler,
        DistributedCacheEntryOptions options)
    {
        _distributedCache = distributedCache;
        _jsonHandler = jsonHandler;
        _distributedCacheEntryOptions = options;
    }

    public async Task AddAsync<T>(string key, T item, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

        var content = _jsonHandler.Serialize(item);
        await _distributedCache.SetStringAsync(key, content, _distributedCacheEntryOptions, cancellationToken);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

        var content = await _distributedCache.GetStringAsync(key, cancellationToken);
        if (content is null)
            return default;

        return _jsonHandler.Desserialize<T>(content);
    }

    public async Task<(bool, T? item)> TryGetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

        var content = await _distributedCache.GetStringAsync(key, cancellationToken);
        if (content is null)
            return (false, default);

        return (true, _jsonHandler.Desserialize<T>(content));
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

        await _distributedCache.RefreshAsync(key, cancellationToken);
    }
}