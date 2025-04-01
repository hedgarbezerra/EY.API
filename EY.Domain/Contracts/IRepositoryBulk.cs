namespace EY.Domain.Contracts;

/// <summary>
///     Defines a contract for performing bulk operations on entities.
///     Provides methods to efficiently update or delete multiple entities in a single operation.
/// </summary>
/// <typeparam name="T">The entity type on which the bulk operations are performed.</typeparam>
public interface IRepositoryBulk<T> where T : class
{
    /// <summary>
    ///     Performs a bulk update operation on the specified entities.
    ///     This method updates multiple entities in a single operation,
    ///     aiming to reduce the number of database round-trips for better performance.
    /// </summary>
    /// <param name="entities">An array of entities to be updated.</param>
    void BulkUpdate(params T[] entities);

    /// <summary>
    ///     Performs a bulk delete operation on the specified entities.
    ///     This method deletes multiple entities in a single operation,
    ///     minimizing the database interactions and improving performance.
    /// </summary>
    /// <param name="entities">An array of entities to be deleted.</param>
    void BulkDelete(params T[] entities);
}