using EY.Domain.Contracts;
using EY.Domain.Models;
using EY.Shared.Attributes;
using EY.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Business.Services
{
    [BindInterface(typeof(IIp2CHttpConsumer))]
    public class Ip2CHttpConsumer : IIp2CHttpConsumer
    {
        private const string ResponseSeparator = ";";

        private readonly IHttpConsumer _httpConsumer;

        public Ip2CHttpConsumer(IHttpConsumer httpConsumer)
        {
            _httpConsumer = httpConsumer;
        }

        public async Task<Result<Ip2CResponse>> GetIp(string ipAddress, CancellationToken cancellationToken = default)
        {
            if (ipAddress.IsValidIpAddress())
                return Result<Ip2CResponse>.Create(false, null, errors: [$"IP Address '{ipAddress}' not valid."]);

            var response = await _httpConsumer.Get<string>(ipAddress, cancellationToken: cancellationToken);
            if (response.Success && response.Data != null)
            {
                var splitedContent = response.Data.Split(ResponseSeparator);

                if (splitedContent.Length == 4)
                {
                    string twoLetterCode = splitedContent[1];
                    string threeLetterCode = splitedContent[2];
                    string name = splitedContent[3];

                    var responseObj = new Ip2CResponse(name, twoLetterCode, threeLetterCode);
                    return Result<Ip2CResponse>.Create(true, responseObj, successes: [$"IP Address '{ipAddress}' response found."]);
                }
                else
                    return Result<Ip2CResponse>.Create(false, null, errors: [$"IP Address '{ipAddress}' response not suitable."]);
            }
            else
                return Result<Ip2CResponse>.Create(false, null, errors: [$"IP Address '{ipAddress}' not found."]);
        }
    }
}
