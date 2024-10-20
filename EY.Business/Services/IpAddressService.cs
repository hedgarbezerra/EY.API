﻿using EY.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Business.Services
{
    public class IpAddressService
    {
        public const string CachePrefix = "IpAddresses_";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCache _redisCache;
        private readonly IIp2CHttpConsumer _httpConsumer;

        public IpAddressService(IUnitOfWork unitOfWork, IRedisCache redisCache, IIp2CHttpConsumer httpConsumer)
        {
            _unitOfWork = unitOfWork;
            _redisCache = redisCache;
            _httpConsumer = httpConsumer;
        }


    }
}
