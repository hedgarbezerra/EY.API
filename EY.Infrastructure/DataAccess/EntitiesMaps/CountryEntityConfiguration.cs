using EY.Domain.Countries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EY.Infrastructure.DataAccess.EntitiesMaps
{
    /// <summary>
    /// Maps <see cref="Country"/> to database table using Entity Framework
    /// </summary>
    public class CountryEntityConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Countries", "dbo");
            builder.HasKey(x => x.Id).HasName("PK_Countries").IsClustered();

            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
            builder.Property(x => x.Name).HasColumnName(@"Name").HasColumnType("varchar(50)").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.TwoLetterCode).HasColumnName(@"TwoLetterCode").HasColumnType("char(2)").IsRequired().IsFixedLength().IsUnicode(false).HasMaxLength(2);
            builder.Property(x => x.ThreeLetterCode).HasColumnName(@"ThreeLetterCode").HasColumnType("char(3)").IsRequired().IsFixedLength().IsUnicode(false).HasMaxLength(3);
            builder.Property(x => x.CreatedAt).HasColumnName(@"CreatedAt").HasColumnType("datetime2").IsRequired();
        }
    }

}
