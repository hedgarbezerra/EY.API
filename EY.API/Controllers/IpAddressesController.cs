using Asp.Versioning;
using EY.API.Attributes;
using EY.Domain.Contracts;
using EY.Domain.Models;
using EY.Domain.Models.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;

namespace EY.API.Controllers;

[OutputCache]
[ApiVersion("1.0", Deprecated = false)]
[Route("api/v{version:apiVersion}/ipaddresses")]
[Route("api/ipaddresses")]
[ApiController]
[EnableRateLimiting(RateLimitOptions.DEFAULT_POLICY)]
public class IpAddressesController : ControllerBase
{
    private readonly IIpAddressesService _ipAddressesService;

    public IpAddressesController(IIpAddressesService ipAddressesService)
    {
        _ipAddressesService = ipAddressesService;
    }

    /// <summary>
    ///     Retrieves an IpAddress entity by its IP string.
    /// </summary>
    /// <param name="ipAddress">The IP address to search for.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
    /// <returns>ActionResult containing the IpAddress entity.</returns>
    [HttpGet("{ipAddress}")]
    public async Task<IActionResult> GetByIpAddress([FromRoute] string ipAddress,
        CancellationToken cancellationToken = default)
    {
        var result = await _ipAddressesService.Get(ipAddress, cancellationToken);
        if (result.Successful)
            return Ok(result);

        return NotFound(result);
    }

    /// <summary>
    ///     Generates a report of IP addresses based on the provided country codes.
    /// </summary>
    /// <param name="countries">Array of two-letter country codes.</param>
    /// <returns>ActionResult containing the IP address report.</returns>
    [HttpGet("report")]
    public IActionResult GetReport([FromQuery] string[] countries)
    {
        var result = _ipAddressesService.Report(countries);
        if (result.Successful)
            return Ok(result);

        return BadRequest(result);
    }

    #region Not part of the project, some additional

    [HttpGet("error")]
    public IActionResult ThrowsException()
    {
        throw new Exception("This is a test exception");
    }

    /// <summary>
    ///     Retrieves a paginated list of IpAddress entities.
    /// </summary>
    /// <param name="pagination">Pagination input including page number, page size, and query.</param>
    /// <returns>ActionResult containing a paginated list of IpAddress entities</returns>
    [IdempotentEndpoint]
    [HttpGet]
    public IActionResult GetPaginated([FromQuery] PaginationInput pagination)
    {
        var result = _ipAddressesService.Get(pagination);
        if (result.Successful)
            return Ok(result);

        return BadRequest(result);
    }

    /// <summary>
    ///     Adds a new IpAddress entity to the database.
    /// </summary>
    /// <param name="ipAddress">The IpAddress entity to add.</param>
    /// <returns>ActionResult indicating success or failure.</returns>
    [HttpPost]
    public IActionResult Add([FromBody] IpAddressInput ipAddress)
    {
        var result = _ipAddressesService.Add(ipAddress);
        if (result.Successful)
            return CreatedAtAction(nameof(GetByIpAddress), new { ipAddress.IpAddress }, ipAddress);

        return BadRequest(result);
    }

    /// <summary>
    ///     Updates an existing IpAddress entity in the database.
    /// </summary>
    /// <param name="ipAddress">The IpAddress entity to update.</param>
    /// <returns>ActionResult indicating success or failure.</returns>
    [HttpPut]
    public IActionResult Update([FromBody] IpAddressInput ipAddress)
    {
        var result = _ipAddressesService.Update(ipAddress);
        if (result.Successful)
            return NoContent();

        return BadRequest(result);
    }


    /// <summary>
    ///     Deletes an IpAddress entity by its IP string.
    /// </summary>
    /// <param name="ipAddress">The IP address of the entity to delete.</param>
    /// <returns>ActionResult indicating success or failure.</returns>
    [HttpDelete("{ipAddress}")]
    public IActionResult Delete([FromRoute] string ipAddress)
    {
        var result = _ipAddressesService.Delete(ipAddress);
        if (result.Successful)
            return NoContent();

        return BadRequest(result);
    }

    #endregion
}