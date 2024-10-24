using EY.Domain.Contracts;
using EY.Domain.Models.Options;
using EY.Infrastructure.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Infrastructure.Cache
{
    public class RedisCacheTests
    {
        private IDistributedCache _distributedCacheMock;
        private IJsonHandler _jsonHandlerMock;
        private DistributedCacheEntryOptions _cacheEntryOptions;
        private RedisCache _redisCache;

        [SetUp]
        public void SetUp()
        {
            _distributedCacheMock = Substitute.For<IDistributedCache>();
            _jsonHandlerMock = Substitute.For<IJsonHandler>();

            _cacheEntryOptions = new DistributedCacheEntryOptions();

            _redisCache = new RedisCache(_distributedCacheMock, _jsonHandlerMock, _cacheEntryOptions);
        }

        [Test]
        public async Task AddAsync_ValidKeyAndItem_ShouldCallSetAsync()
        {
            // Arrange
            var key = "testKey";
            var item = new { Id = 1, Name = "Test" };
            var serializedItem = "serializedContent";
            var bytes = Encoding.UTF8.GetBytes(serializedItem);

            _jsonHandlerMock.Serialize(item).Returns(serializedItem);

            // Act
            await _redisCache.AddAsync(key, item);

            // Assert
            await _distributedCacheMock.Received(1).SetAsync(
                key,
                Arg.Is<byte[]>(b => b.SequenceEqual(bytes)),
                _cacheEntryOptions,
                Arg.Any<CancellationToken>()
            );
        }

        [Test]
        public void AddAsync_NullOrEmptyKey_ShouldThrowArgumentException()
        {
            // Arrange
            string key = null;
            var item = new { Id = 1, Name = "Test" };

            // Act
            Func<Task> act = async () => await _redisCache.AddAsync(key, item);

            // Assert
            act.Should().ThrowAsync<ArgumentException>().WithMessage("*key*");
        }

        [Test]
        public async Task GetAsync_ValidKey_ShouldReturnDeserializedItem()
        {
            // Arrange
            var key = "testKey";
            var json = "{\"Id\":1,\"Name\":\"Test\"}";
            var expectedItem = new { Id = 1, Name = "Test" };
            var bytes = Encoding.UTF8.GetBytes(json);

            // Mock GetAsync to return the byte array (simulating data in cache)
            _distributedCacheMock.GetAsync(key, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(bytes));

            _jsonHandlerMock.Desserialize<object>(json).Returns(expectedItem);

            // Act
            var result = await _redisCache.GetAsync<object>(key);

            // Assert
            result.Should().BeEquivalentTo(expectedItem);
        }

        [Test]
        public async Task GetAsync_KeyNotFound_ShouldReturnNull()
        {
            // Arrange
            var key = "testKey";

            // Mock GetAsync to return null (simulating key not found)
            _distributedCacheMock.GetAsync(key, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<byte[]>(null));

            // Act
            var result = await _redisCache.GetAsync<object>(key);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task TryGetAsync_ValidKey_ShouldReturnTrueAndItem()
        {
            // Arrange
            var key = "testKey";
            var json = "{\"Id\":1,\"Name\":\"Test\"}";
            var expectedItem = new { Id = 1, Name = "Test" };
            var bytes = Encoding.UTF8.GetBytes(json);

            // Mock GetAsync to return the byte array
            _distributedCacheMock.GetAsync(key, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(bytes));

            _jsonHandlerMock.Desserialize<object>(json).Returns(expectedItem);

            // Act
            var (found, item) = await _redisCache.TryGetAsync<object>(key);

            // Assert
            found.Should().BeTrue();
            item.Should().BeEquivalentTo(expectedItem);
        }

        [Test]
        public async Task TryGetAsync_KeyNotFound_ShouldReturnFalseAndNull()
        {
            // Arrange
            var key = "testKey";

            // Mock GetAsync to return null
            _distributedCacheMock.GetAsync(key, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<byte[]>(null));

            // Act
            var (found, item) = await _redisCache.TryGetAsync<object>(key);

            // Assert
            found.Should().BeFalse();
            item.Should().BeNull();
        }

        [Test]
        public async Task RemoveAsync_ValidKey_ShouldCallRefreshAsync()
        {
            // Arrange
            var key = "testKey";

            // Act
            await _redisCache.RemoveAsync(key);

            // Assert
            await _distributedCacheMock.Received(1).RefreshAsync(key, Arg.Any<CancellationToken>());
        }

        [Test]
        public void RemoveAsync_NullOrEmptyKey_ShouldThrowArgumentException()
        {
            // Arrange
            string key = null;

            // Act
            Func<Task> act = async () => await _redisCache.RemoveAsync(key);

            // Assert
            act.Should().ThrowAsync<ArgumentException>().WithMessage("*key*");
        }
    }
}
