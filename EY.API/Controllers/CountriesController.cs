using Asp.Versioning;
using EY.Domain.Contracts;
using EY.Domain.Models;
using EY.Domain.Models.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;

namespace EY.API.Controllers
{
    [OutputCache]
    [ApiVersion("1.0", Deprecated = false)]
    [Route("api/v{version:apiVersion}/countries")]
    [Route("api/countries")]
    [ApiController]
    [EnableRateLimiting(RateLimitOptions.DEFAULT_POLICY)]
    public class CountriesController : ControllerBase
    {
        private readonly ICountriesService _countriesService;

        public CountriesController(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        /// <summary>
        /// Retrieves a Country entity by its three-letter code.
        /// </summary>
        /// <param name="threeLetterCode">The three-letter country code to search for.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
        /// <returns>ActionResult containing the Country entity.</returns>
        [HttpGet("{threeLetterCode:maxlength(3)}")]
        public async Task<IActionResult> GetByThreeLetterCode([FromRoute] string threeLetterCode, CancellationToken cancellationToken = default)
        {
            var result = await _countriesService.Get(threeLetterCode, cancellationToken);
            if (result.Successful)
                return Ok(result);

            return NotFound(result);
        }

        /// <summary>
        /// Retrieves a paginated list of Country entities.
        /// </summary>
        /// <param name="pagination">Pagination input including page number, page size, and query.</param>
        /// <returns>ActionResult containing a paginated list of Country entities.</returns>
        [HttpGet]
        public ActionResult GetPaginated([FromQuery] PaginationInput pagination)
        {
            var result = _countriesService.Get(pagination);
            if (result.Successful)
                return Ok(result);

            return BadRequest(result);
        }

        #region Not part of the project, some additional endpoints
        /// <summary>
        /// Adds a new Country entity to the database.
        /// </summary>
        /// <param name="country">The Country entity to add.</param>
        /// <returns>ActionResult indicating success or failure.</returns>
        [HttpPost]
        public IActionResult Add([FromBody] CountryInput country)
        {
            var result = _countriesService.Add(country);
            if (result.Successful)
                return CreatedAtAction(nameof(GetByThreeLetterCode), new { country.ThreeLetterCode }, country);

            return BadRequest(result);
        }

        /// <summary>
        /// Updates an existing Country entity in the database.
        /// </summary>
        /// <param name="country">The Country entity with updated information.</param>
        /// <returns>ActionResult indicating success or failure.</returns>
        [HttpPut]
        public IActionResult Update([FromBody] CountryInput country)
        {
            var result = _countriesService.Update(country);
            if (result.Successful)
                return NoContent();

            return BadRequest(result);
        }

        /// <summary>
        /// Deletes a Country entity by its three-letter code.
        /// </summary>
        /// <param name="threeLetterCode">The three-letter country code of the entity to delete.</param>
        /// <returns>ActionResult indicating success or failure.</returns>
        [HttpDelete("{threeLetterCode}")]
        public IActionResult Delete([FromRoute] string threeLetterCode)
        {
            var result = _countriesService.Delete(threeLetterCode);
            if (result.Successful)
                return NoContent();

            return BadRequest(result);
        }
        #endregion
    }
}
