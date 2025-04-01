using EY.Domain.Countries;
using EY.Domain.IpAddresses;
using EY.Infrastructure.DataAccess.EntitiesMaps;
using Microsoft.EntityFrameworkCore;

namespace EY.Infrastructure.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    ///     Code representation for database table for Countries
    /// </summary>
    public DbSet<Country> Countries { get; set; }

    /// <summary>
    ///     Code representation for database table for IP Addresses
    /// </summary>
    public DbSet<IpAddress> IpAddresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CountryEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new IpAddressEntityTypeConfiguration());
    }
}