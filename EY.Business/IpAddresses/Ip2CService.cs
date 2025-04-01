using EY.Domain.Contracts;
using EY.Domain.IpAddresses;
using EY.Domain.Models;
using EY.Shared.Attributes;
using EY.Shared.Extensions;

namespace EY.Business.IpAddresses;

[BindInterface(typeof(IIp2CService))]
public class Ip2CService : IIp2CService
{
    private const string ResponseSeparator = ";";
    private const string Ip2CBaseURL = "http://ip2c.org";

    private readonly IHttpConsumer _httpConsumer;

    public Ip2CService(IHttpConsumer httpConsumer)
    {
        _httpConsumer = httpConsumer;
    }

    public async Task<Result<Ip2CResponse>> GetIp(string ipAddress, CancellationToken cancellationToken = default)
    {
        if (!ipAddress.IsValidIpAddress())
            return IpAddressesResults.Errors.IpNotValid(ipAddress);

        var response =
            await _httpConsumer.Get<string>($"{Ip2CBaseURL}/{ipAddress}", cancellationToken: cancellationToken);
        if (response.Successful && response.Data != null)
        {
            var splitedContent = response.Data.Split(ResponseSeparator);

            if (splitedContent.Length == 4)
            {
                var twoLetterCode = splitedContent[1];
                var threeLetterCode = splitedContent[2];
                var name = splitedContent[3];

                var responseObj = new Ip2CResponse(ipAddress, name, twoLetterCode, threeLetterCode);
                return Result<Ip2CResponse>.Success(responseObj, $"IP Address '{ipAddress}' response found.");
            }

            return Result<Ip2CResponse>.Failure($"IP Address '{ipAddress}' response not suitable.");
        }

        return Result<Ip2CResponse>.Failure($"IP Address '{ipAddress}' not found.");
    }
}