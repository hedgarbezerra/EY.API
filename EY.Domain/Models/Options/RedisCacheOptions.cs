using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Models.Options
{
    public class RedisCacheOptions
    {
        public const string SettingsKey = "Redis";

        /// <summary>
        /// Name of the Redis Instance
        /// </summary>
        public required string Instance { get; init; }
        /// <summary>
        /// Redis Connection String
        /// </summary>
        public required string ConnectionString { get; init; }
        /// <summary>
        /// How long items will be cached in seconds
        /// </summary>
        public required int CacheExpiracyInSeconds { get; init; }
    }
}
