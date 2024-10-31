using EY.Domain.Countries;
using EY.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Contracts
{
    /// <summary>
    /// Interface responsible for CRUD operations on Country entities in the database,
    /// as well as generating reports based on country codes.
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Retrieves an Country entity by its three letters code string.
        /// The method first checks the cache; if not found, it queries the database.
        /// If the entity is still not found, it fetches the data from an external service.
        /// </summary>
        /// <param name="threeLetterCode">The three letters length country's code.</param>
        /// <returns>A Result containing the Country entity or an error.</returns>
        Task<Result<Country>> Get(string threeLetterCode, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of Country entities.
        /// This method only looks into database.
        /// </summary>
        /// <param name="pagination">Pagination information (page number, page size and query).</param>
        /// <returns>A Result containing a paginated list of Country entities or an error.</returns>
        Result<PaginatedList<Country>> Get(PaginationInput pagination);

        /// <summary>
        /// Adds a new Country entity to the database.
        /// </summary>
        /// <param name="country">The Country entity to add.</param>
        /// <returns>A Result indicating the success or failure of the operation.</returns>
        Result Add(CountryInput country);

        /// <summary>
        /// Updates an existing Country entity in the database.
        /// </summary>
        /// <param name="country">The Country entity with updated information.</param>
        /// <returns>A Result indicating the success or failure of the operation.</returns>
        Result Update(CountryInput country);

        /// <summary>
        /// Deletes an Country entity from the database by its three letters code string.
        /// </summary>
        /// <param name="threeLetterCode">The three letters length country's code to delete.</param>
        /// <returns>A Result indicating the success or failure of the operation.</returns>
        Result Delete(string threeLetterCode);
    }
}
