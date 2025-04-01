using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace EY.Shared.Contracts;

/// <summary>
///     Defines a contract for executing batch update and delete operations on entities using Entity Framework.
///     Uses expression-based queries to perform efficient updates and deletions in bulk.
/// </summary>
/// <typeparam name="T">The entity type on which the batch operations are performed.</typeparam>
public interface ISqlBatchOperation<T>
{
    /// <summary>
    ///     Executes a batch update operation on entities that match the specified query condition.
    ///     Uses a set of property assignments to update multiple records in a single operation,
    ///     improving performance by reducing database round-trips.
    /// </summary>
    /// <param name="query">An expression that defines the filter condition to identify the entities to update.</param>
    /// <param name="setPropertyCalls">An expression that specifies the properties to update and their new values.</param>
    /// <returns>The number of rows affected by the update operation.</returns>
    int ExecuteUpdate(Expression<Func<T, bool>> query,
        Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls);

    /// <summary>
    ///     Executes a batch delete operation on entities that match the specified query condition.
    ///     Deletes multiple records in a single operation based on the filter criteria, optimizing performance
    ///     by minimizing the number of database interactions.
    /// </summary>
    /// <param name="query">An expression that defines the filter condition to identify the entities to delete.</param>
    /// <returns>The number of rows affected by the delete operation.</returns>
    int ExecuteDelete(Expression<Func<T, bool>> query);
}