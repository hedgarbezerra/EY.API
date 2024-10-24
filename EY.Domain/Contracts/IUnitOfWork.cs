using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Contracts
{
    /// <summary>
    /// This pattern helps to manage transactions across multiple repositories in a single business operation.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Gets the repository for the specified entity type.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <returns>An instance of a repository for the specified entity type.</returns>
        IRepository<T> Repository<T>() where T : class;

        /// <summary>
        /// Commits all changes made within this unit of work.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        int Commit();
    }
}
