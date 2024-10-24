using EY.Domain.Entities;
using EY.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Contracts
{
    /// <summary>
    /// Interface responsible for CRUD operations on IpAddress entities in the database,
    /// as well as generating reports based on country codes.
    /// </summary>
    public interface IIpAddressesService
    {
        /// <summary>
        /// Retrieves an IpAddress entity by its IP string.
        /// The method first checks the cache; if not found, it queries the database.
        /// If the entity is still not found, it fetches the data from an external service.
        /// </summary>
        /// <param name="ipAddress">The IP address to search for.</param>
        /// <returns>A Result containing the IpAddress entity or an error.</returns>
        Task<Result<Ip2CResponse>> Get(string ipAddress, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of IpAddress entities.
        /// This method only looks into the database.
        /// </summary>
        /// <param name="pagination">Pagination information (page number, page size and query).</param>
        /// <returns>A Result containing a paginated list of IpAddress entities or an error.</returns>
        Result<PaginatedList<Ip2CResponse>> Get(PaginationInput pagination);

        /// <summary>
        /// Adds a new IpAddress entity to the database.
        /// </summary>
        /// <param name="ipAddress">The IpAddress entity to add.</param>
        /// <returns>A Result indicating the success or failure of the operation.</returns>
        Result Add(IpAddressInput ipAddress);

        /// <summary>
        /// Updates an existing IpAddress entity in the database.
        /// </summary>
        /// <param name="ipAddress">The IpAddress entity with updated information.</param>
        /// <returns>A Result indicating the success or failure of the operation.</returns>
        Result Update(IpAddressInput ipAddress);

        /// <summary>
        /// Deletes an IpAddress entity from the database by its IP string.
        /// </summary>
        /// <param name="ipAddress">The IP address of the entity to delete.</param>
        /// <returns>A Result indicating the success or failure of the operation.</returns>
        Result Delete(string ipAddress);

        /// <summary>
        /// Generates a report of IP addresses based on the provided country codes.
        /// </summary>
        /// <param name="twoLetterCountriesCodes">An array of two-letter country codes to filter the IPs by.</param>
        /// <returns>A Result containing a list of IpAddressReportItems or an error.</returns>
        Result<List<IpAddressReportItem>> Report(params string[] twoLetterCountriesCodes);
    }

}
