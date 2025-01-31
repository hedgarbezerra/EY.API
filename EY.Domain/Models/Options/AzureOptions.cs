﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Models.Options
{
    public class AzureOptions
    {
        public const string SettingsKey = "Azure";

        public required AppConfigurationsOptions AppConfigurations { get; init; }
    }

    public class AppConfigurationsOptions
    {
        /// <summary>
        /// Connection string for the App Configurations on Azure
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public required string ConnectionString { get; init; }
        /// <summary>
        /// Sentinel Key to keep cached keys updated
        /// </summary>
        public required string CacheSentinel { get; init; }
        
        /// <summary>
        /// Time in seconds for the cache expiracy
        /// </summary>
        public int? CacheExpiracySeconds { get; init; }
    }
}
