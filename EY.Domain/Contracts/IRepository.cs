using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace EY.Domain.Contracts
{
    /// <summary>
    /// Generic repository interface defining basic data operations for entities of type T.
    /// Provides methods to add, delete, update, and retrieve entities from the data source.
    /// </summary>
    /// <typeparam name="T">The type of the entity for which the repository is handling data operations.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Adds a new entity to the data source.
        /// </summary>
        /// <param name="obj">The entity to add.</param>
        void Add(T obj);

        /// <summary>
        /// Deletes an entity from the data source using its unique key.
        /// </summary>
        /// <param name="key">The unique identifier of the entity to delete.</param>
        void Delete(int key);

        /// <summary>
        /// Retrieves all entities of type T from the data source.
        /// </summary>
        /// <param name="includeRelated">If should load related entities(Foreign Keys).</param>
        /// <returns>An IQueryable of all entities.</returns>
        IQueryable<T> Get(bool includeRelated = false);

        /// <summary>
        /// Retrieves entities of type T that satisfy a specified filter condition.
        /// </summary>
        /// <param name="filter">An expression specifying the condition to filter the entities.</param>
        /// <param name="includeRelated">If should load related entities(Foreign Keys).</param>
        /// <returns>An IQueryable of entities that match the filter condition.</returns>
        IQueryable<T> Get(Expression<Func<T, bool>> filter, bool includeRelated = false);

        /// <summary>
        /// Retrieves a single entity of type T using its unique key.
        /// </summary>
        /// <param name="key">The unique identifier of the entity to retrieve.</param>
        /// <returns>The entity with the specified key, or null if not found.</returns>
        T? Get(int key);

        /// <summary>
        /// Updates an existing entity in the data source.
        /// </summary>
        /// <param name="obj">The entity with updated values.</param>
        void Update(T obj);
    }

}
