using EY.Domain.Countries;
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
            //Arrange
            List<string> countryProperties = [
                nameof(Country.Id),
                nameof(Country.Name),
                nameof(Country.TwoLetterCode),
                nameof(Country.ThreeLetterCode),
                nameof(Country.CreatedAt)];
            var sut = new CountryEntityConfiguration();
            var builder = EntityConfiguratationsHelper.GetEntityBuilder<Country>();

            //Act
            sut.Configure(builder);

            var meta = builder.Metadata;
            var properties = meta.GetDeclaredProperties();
            var propertiesNames = properties.Select(p => p.Name).ToList();

            //Assert
            meta.Name.Should().Be(nameof(Country));
            propertiesNames.Should().NotBeEmpty();
            propertiesNames.Count().Should().Be(countryProperties.Count());
            propertiesNames.Should().BeEquivalentTo(propertiesNames);

            var mappedId = properties.First(p => p.Name == nameof(Country.Id));
            mappedId.IsKey().Should().BeTrue();
        }

    }
}
