using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using EY.Domain.IpAddresses;

namespace EY.Domain.Countries
{
    //TODO: refatorar para termos um maior nível de abstração e também ter um domínio mais bem definido que não depende de primitivos.
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
