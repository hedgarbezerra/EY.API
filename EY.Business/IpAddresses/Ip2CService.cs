using EY.Domain.Contracts;
using EY.Domain.IpAddresses;
using EY.Domain.Models;
using EY.Shared.Attributes;
using EY.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Business.IpAddresses
{
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

            var response = await _httpConsumer.Get<string>($"{Ip2CBaseURL}/{ipAddress}", cancellationToken: cancellationToken);
            if (response.Successful && response.Data != null)
            {
                var splitedContent = response.Data.Split(ResponseSeparator);

                if (splitedContent.Length == 4)
                {
                    string twoLetterCode = splitedContent[1];
                    string threeLetterCode = splitedContent[2];
                    string name = splitedContent[3];

                    var responseObj = new Ip2CResponse(ipAddress, name, twoLetterCode, threeLetterCode);
                    return Result<Ip2CResponse>.Success(responseObj, [$"IP Address '{ipAddress}' response found."]);
                }
                else
                    return Result<Ip2CResponse>.Failure([$"IP Address '{ipAddress}' response not suitable."]);
            }
            else
                return Result<Ip2CResponse>.Failure([$"IP Address '{ipAddress}' not found."]);
        }
    }
}
