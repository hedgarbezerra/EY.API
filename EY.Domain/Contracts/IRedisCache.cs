using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EY.Domain.Contracts
{
    /// <summary>
    /// Interface representing a cache service using Redis.
    /// Provides methods for retrieving, adding, and attempting to get items from the cache.
    /// </summary>
    public interface IRedisCache
    {
        /// <summary>
        /// Attempts to retrieve an item from the cache by the specified key.
        /// If the item exists, it is returned via the out parameter.
        /// </summary>
        /// <typeparam name="T">The type of the item to retrieve.</typeparam>
        /// <param name="key">The key of the item in the cache.</param>
        /// <param name="item">The retrieved item if found in the cache, otherwise the default value of the type.</param>
        /// <returns>The task result contains a boolean indicating whether the item was found in the cache.</returns>
        Task<(bool, T? item)> TryGetAsync<T>(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves an item from the cache by the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the item to retrieve.</typeparam>
        /// <param name="key">The key of the item in the cache.</param>
        /// <returns>The task result contains the item retrieved from the cache.</returns>
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes an item from the cache by the specified key.
        /// </summary>
        /// <param name="key">The key of the item in the cache.</param>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds an item to the cache with the specified key.
        /// If the key already exists, the item is updated.
        /// </summary>
        /// <typeparam name="T">The type of the item to add to the cache.</typeparam>
        /// <param name="key">The key under which the item will be stored in the cache.</param>
        /// <param name="item">The item to add to the cache.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddAsync<T>(string key, T item, CancellationToken cancellationToken = default);
    }
}
