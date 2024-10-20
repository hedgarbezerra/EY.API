using EY.Domain.Models.Options;
using EY.Infrastructure.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Infrastructure.Cache
{
    [TestFixture]
    public class RedisCacheTests
    {
        private IDistributedCache _distributedCache;
        private RedisCache _redisCache;
        private IOptions<RedisCacheOptions> _cacheOptions;

        [SetUp]
        public void SetUp()
        {
            _distributedCache = Substitute.For<IDistributedCache>();
            _cacheOptions = Options.Create(new RedisCacheOptions { CacheExpiracyInSeconds = 300, ConnectionString = "validConnection", Instance = "Instance" });
            _redisCache = new RedisCache(_distributedCache, _cacheOptions);
        }

        [Test]
        public async Task AddAsync_ShouldStoreItemInCache()
        {
            // Arrange
            var key = "test-key";
            var item = new { Id = 1, Name = "Test Item" };
            var serializedItem = JsonConvert.SerializeObject(item);

            // Act
            await _redisCache.AddAsync(key, item);

            // Assert
            await _distributedCache.Received(1).SetStringAsync(
                key,
                serializedItem,
                Arg.Is<DistributedCacheEntryOptions>(options =>
                    options.AbsoluteExpirationRelativeToNow == TimeSpan.FromSeconds(_cacheOptions.Value.CacheExpiracyInSeconds)));
        }

        [Test]
        public async Task GetAsync_ShouldReturnItemIfExistsInCache()
        {
            // Arrange
            var key = "test-key";
            var item = new { Id = 1, Name = "Test Item" };
            var serializedItem = JsonConvert.SerializeObject(item);

            _distributedCache.GetStringAsync(key).Returns(serializedItem);

            // Act
            var result = await _redisCache.GetAsync<dynamic>(key);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Test Item");
        }

        [Test]
        public async Task GetAsync_ShouldReturnDefaultIfItemDoesNotExistInCache()
        {
            // Arrange
            var key = "non-existent-key";
            _distributedCache.GetStringAsync(key).Returns((string)null);

            // Act
            var result = await _redisCache.GetAsync<dynamic>(key);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task TryGetAsync_ShouldReturnTrueAndItemIfExistsInCache()
        {
            // Arrange
            var key = "test-key";
            var item = new { Id = 1, Name = "Test Item" };
            var serializedItem = JsonConvert.SerializeObject(item);

            _distributedCache.GetStringAsync(key).Returns(serializedItem);

            // Act
            var (found, result) = await _redisCache.TryGetAsync<dynamic>(key);

            // Assert
            found.Should().BeTrue();
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Test Item");
        }

        [Test]
        public async Task TryGetAsync_ShouldReturnFalseIfItemDoesNotExistInCache()
        {
            // Arrange
            var key = "non-existent-key";
            _distributedCache.GetStringAsync(key).Returns((string)null);

            // Act
            var (found, result) = await _redisCache.TryGetAsync<dynamic>(key);

            // Assert
            found.Should().BeFalse();
            result.Should().BeNull();
        }

        [Test]
        public async Task RemoveAsync_ShouldRefreshItemInCache()
        {
            // Arrange
            var key = "test-key";

            // Act
            await _redisCache.RemoveAsync(key);

            // Assert
            await _distributedCache.Received(1).RefreshAsync(key);
        }
    }
}
