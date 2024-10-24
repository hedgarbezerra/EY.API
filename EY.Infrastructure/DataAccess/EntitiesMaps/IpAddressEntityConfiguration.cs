using EY.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EY.Infrastructure.DataAccess.EntitiesMaps
{
    /// <summary>
    /// Maps <see cref="IpAddress"/> to database table using Entity Framework
    /// </summary>
    public class IpAddressEntityConfiguration : IEntityTypeConfiguration<IpAddress>
    {
        public void Configure(EntityTypeBuilder<IpAddress> builder)
        {
            builder.ToTable("IPAddresses", "dbo");
            builder.HasKey(x => x.Id).HasName("PK_IPAddresses").IsClustered();

            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.CountryId).HasColumnName(@"CountryId").HasColumnType("int").IsRequired();
            builder.Property(x => x.Ip).HasColumnName(@"IP").HasColumnType("varchar(15)").IsRequired().IsUnicode(false).HasMaxLength(15);
            builder.Property(x => x.CreatedAt).HasColumnName(@"CreatedAt").HasColumnType("datetime2").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnName(@"UpdatedAt").HasColumnType("datetime2").IsRequired();

            builder.HasOne(a => a.Country).WithMany(b => b.IpAddresses).HasForeignKey(c => c.CountryId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_IPAddresses_Countries");

            builder.HasIndex(x => x.Ip).HasDatabaseName("IX_IPAddresses").IsUnique();
            builder.Navigation(p => p.Country)
                .AutoInclude();
        }
    }

}
