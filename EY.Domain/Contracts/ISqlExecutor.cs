using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Contracts
{
    /// <summary>
    /// Defines a contract for executing raw SQL queries using Entity Framework.
    /// Provides methods for executing SQL queries and returning results as strongly-typed entities.
    /// </summary>
    /// <typeparam name="T">The entity type to be used in the query execution.</typeparam>
    public interface ISqlExecutor
    {
        /// <summary>
        /// Executes a SQL query using a <see cref="FormattableString"/> to ensure type safety and parameterization.
        /// This approach helps prevent SQL injection attacks by treating interpolated values as parameters.
        /// </summary>
        /// <param name="query">The SQL query as a <see cref="FormattableString"/>.</param>
        /// <returns>An <see cref="IQueryable{T}"/> representing the results of the query.</returns>
        IQueryable<T> Query<T>(FormattableString query);

        /// <summary>
        /// Executes a SQL query using a raw SQL string and parameter values.
        /// The parameters are automatically handled by Entity Framework to ensure they are properly escaped, reducing the risk of SQL injection attacks.
        /// </summary>
        /// <param name="sql">The raw SQL query string to execute.</param>
        /// <param name="parameters">An array of parameters to be used in the query.</param>
        /// <returns>An <see cref="IQueryable{T}"/> representing the results of the query.</returns>
        IQueryable<T> Query<T>(string query, object parameters);

        /// <summary>
        /// Executes a SQL command that does not return a result set (e.g., INSERT, UPDATE, DELETE statements).
        /// Uses a <see cref="FormattableString"/> to ensure that values are properly parameterized, preventing SQL injection attacks.
        /// </summary>
        /// <param name="query">The SQL command to execute as a <see cref="FormattableString"/>.</param>
        /// <returns>The number of rows affected by the SQL command.</returns>
        int Execute(FormattableString query);
    }

}
