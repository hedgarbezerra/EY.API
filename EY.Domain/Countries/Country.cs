using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using EY.Domain.IpAddresses;

namespace EY.Domain.Countries
{
    public class Country
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string TwoLetterCode { get; set; }
        public required string ThreeLetterCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public virtual ICollection<IpAddress> IpAddresses { get; set; } = new List<IpAddress>();
    }

}
