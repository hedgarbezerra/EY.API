using EY.Domain.Entities;
using EY.Infrastructure.DataAccess.EntitiesMaps;
using Microsoft.EntityFrameworkCore;

namespace EY.Infrastructure.DataAccess
{
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Code representation for database table for Countries
        /// </summary>
        public DbSet<Country> Countries { get; set; }

        /// <summary>
        /// Code representation for database table for IP Addresses
        /// </summary>
        public DbSet<IpAddress> IpAddresses { get; set; } 

        public AppDbContext(DbContextOptions options) : base(options)
        {            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CountryEntityConfiguration());
            modelBuilder.ApplyConfiguration(new IpAddressEntityConfiguration());
        }
    }
}
