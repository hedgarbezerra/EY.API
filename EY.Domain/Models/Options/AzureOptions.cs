using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Models.Options
{
    public class AzureOptions
    {
        public const string SettingsKey = "Azure";

        public required AppConfigurations AppConfigurations { get; set; }
    }

    public class AppConfigurations
    {
        /// <summary>
        /// Connection string for the App Configurations on Azure
        /// </summary>
        public required string ConnectionString { get; set; }
        /// <summary>
        /// Sentinel Key to keep cached keys updated
        /// </summary>
        public required string CacheSentinel { get; set; }
        /// <summary>
        /// Time in seconds for the cache expiracy
        /// </summary>
        public int? CacheExpiracySeconds { get; set; }
    }
}
