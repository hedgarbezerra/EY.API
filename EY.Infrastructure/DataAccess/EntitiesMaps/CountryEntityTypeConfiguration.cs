using EY.Domain.Countries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EY.Infrastructure.DataAccess.EntitiesMaps;

/// <summary>
///     Maps <see cref="Country" /> to database table using Entity Framework
/// </summary>
public class CountryEntityTypeConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Countries", "dbo");
        builder.HasKey(x => x.Id)
            .HasName("PK_Countries")
            .IsClustered();

        builder.Property(x => x.Id)
            .HasColumnName(@"Id")
            .HasColumnType("int")
            .IsRequired()
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();
        builder.Property(x => x.Name)
            .HasColumnName(@"Name")
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .IsRequired()
            .IsUnicode(false);
        builder.Property(x => x.TwoLetterCode)
            .HasColumnName(@"TwoLetterCode")
            .HasColumnType("char")
            .HasMaxLength(2)
            .IsRequired()
            .IsFixedLength()
            .IsUnicode(false);
        builder.Property(x => x.ThreeLetterCode)
            .HasColumnName(@"ThreeLetterCode")
            .HasColumnType("char")
            .HasMaxLength(3)
            .IsRequired()
            .IsFixedLength()
            .IsUnicode(false);
        builder.Property(x => x.CreatedAt)
            .HasColumnName(@"CreatedAt")
            .HasColumnType("datetime2")
            .IsRequired();
    }
}