using EY.Domain.Models;

namespace EY.Domain.Contracts;

public interface IIp2CService
{
    /// <summary>
    ///     Gets country information on the IP Address provided
    /// </summary>
    /// <param name="ipAddress">IP Address </param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
    /// <returns>The task result contains a Result object, which includes the response from the external service.</returns>
    Task<Result<Ip2CResponse>> GetIp(string ipAddress, CancellationToken cancellationToken = default);
}