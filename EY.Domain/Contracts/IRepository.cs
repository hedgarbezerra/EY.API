using System.Linq.Expressions;

namespace EY.Domain.Contracts;

/// <summary>
///     Generic repository interface defining basic data operations for entities of type T.
///     Provides methods to add, delete, update, and retrieve entities from the data source.
/// </summary>
/// <typeparam name="T">The type of the entity for which the repository is handling data operations.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    ///     Retrieves all entities of type T from the data source.
    /// </summary>
    /// <returns>An IQueryable of all entities.</returns>
    IQueryable<T> Get();

    /// <summary>
    ///     Retrieves entities of type T that satisfy a specified filter condition.
    /// </summary>
    /// <param name="filter">An expression specifying the condition to filter the entities.</param>
    /// <returns>An IQueryable of entities that match the filter condition.</returns>
    IQueryable<T> Get(Expression<Func<T, bool>> filter);

    /// <summary>
    ///     Retrieves a single entity of type T using its unique key.
    /// </summary>
    /// <param name="key">The unique identifier of the entity to retrieve.</param>
    /// <returns>The entity with the specified key, or null if not found.</returns>
    T? Get(int key);

    /// <summary>
    ///     Adds a new entity to the data source.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    void Add(T entity);

    /// <summary>
    ///     Deletes an entity from the data source using its unique key.
    /// </summary>
    /// <param name="key">The unique identifier of the entity to delete.</param>
    void Delete(int key);

    /// <summary>
    ///     Updates an existing entity in the data source.
    /// </summary>
    /// <param name="entity">The entity with updated values.</param>
    void Update(T entity);
}