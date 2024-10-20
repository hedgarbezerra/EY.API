using EY.Domain.Entities;
using EY.Infrastructure.DataAccess.EntitiesMaps;
using EY.Tests.Helpers;

namespace EY.Tests.Infrastructure.DataAccess.EntitiesMaps
{
    [TestFixture]
    internal class CountriesConfigurationTests
    {
        [Test]
        public void Configure()
        {
            var countryProperties = new[]{
                nameof(Country.Id),
                nameof(Country.Name),
                nameof(Country.TwoLetterCode),
                nameof(Country.ThreeLetterCode),
                nameof(Country.CreatedAt),
                };
            var sut = new CountryEntityConfiguration();
            var builder = EntityConfiguratationsHelper.GetEntityBuilder<Country>();

            sut.Configure(builder);

            var meta = builder.Metadata;
            var properties = meta.GetDeclaredProperties();
            var propertiesNames = properties.Select(p => p.Name);

            meta.Name.Should().Be(nameof(Country));
            properties.Should().NotBeEmpty();
            countryProperties.Length.Should().Be(properties.Count());
            CollectionAssert.AreEqual(countryProperties, propertiesNames);

            var mappedId = properties.First(p => p.Name == nameof(Country.Id));
            Assert.True(mappedId.IsKey());
        }

    }
}
