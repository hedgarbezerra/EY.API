using EY.Domain.Countries;
using EY.Domain.IpAddresses;
using EY.Infrastructure.DataAccess.EntitiesMaps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

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

        public AppDbContext()
        {  }
        public AppDbContext(DbContextOptions options) : base(options)
        {            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
            .HasDbFunction(typeof(AppDbContext).GetMethod(nameof(Soundex)))
            .HasTranslation(args => new SqlFunctionExpression("SOUNDEX", args, true,[false], typeof(string), null));

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CountryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new IpAddressEntityTypeConfiguration());
        }
        public static string Soundex(string input)
        {
            // This method is a placeholder for the SQL function
            throw new NotSupportedException();
        }
    }
}
