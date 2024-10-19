using EY.Domain.Contracts;
using EY.Domain.Models.Options;
using EY.Shared.Attributes;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Infrastructure.Cache
{
    [BindInterface(typeof(IRedisCache))]
    public class RedisCache : IRedisCache
    {
        private readonly IDistributedCache _distributedCache;
        private readonly DistributedCacheEntryOptions _distributedCacheEntryOptions;
        public RedisCache(IDistributedCache distributedCache, IOptions<RedisCacheOptions> cacheOptions)
        {
            _distributedCache = distributedCache;
            _distributedCacheEntryOptions = new() 
            { 
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheOptions.Value.CacheExpiracyInSeconds) 
            };
        }

        public async Task AddAsync<T>(string key, T item)
        {
            var content = JsonConvert.SerializeObject(item);
            await _distributedCache.SetStringAsync(key, content, _distributedCacheEntryOptions);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var content = await _distributedCache.GetStringAsync(key);
            if (content is null)
                return default;

            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<(bool, T item)> TryGetAsync<T>(string key)
        {
            var content = await _distributedCache.GetStringAsync(key);
            if (content is null)
                return (false, default);

            return (true, JsonConvert.DeserializeObject<T>(content));
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RefreshAsync(key);
        }
    }
}
