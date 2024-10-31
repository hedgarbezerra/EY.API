using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using EY.Domain.Countries;

namespace EY.Domain.IpAddresses
{
    public class IpAddress
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string Ip { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public virtual Country Country { get; set; }
    }
}