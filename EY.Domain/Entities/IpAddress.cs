using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EY.Domain.Entities
{
    public class IpAddress
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string Ip { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 
        public Country Country { get; set; } 

        public IpAddress()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}