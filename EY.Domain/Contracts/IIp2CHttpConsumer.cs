using EY.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Contracts
{
    public interface IIp2CHttpConsumer
    {
        Task<Result<Ip2CResponse>> GetIp(string ipAddress);
    }
}
