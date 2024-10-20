using EY.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Business.Services
{
    public class CountriesService
    {
        public const string CachePrefix = "Countries_";
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCache _redisCache;

        public CountriesService(IUnitOfWork unitOfWork, IRedisCache redisCache)
        {
            _unitOfWork = unitOfWork;
            _redisCache = redisCache;
        }
    }
}
