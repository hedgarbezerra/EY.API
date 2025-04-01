using EY.Domain.IpAddresses;
using EY.Infrastructure.DataAccess.EntitiesMaps;
using EY.Tests.Helpers;

namespace EY.Tests.Infrastructure.DataAccess.EntitiesMaps;

[TestFixture]
internal class IpAddressesConfigurationTests
{
    [Test]
    public void Configure()
    {
        //Arrange
        List<string> ipAddressProperties =
        [
            nameof(IpAddress.Id), nameof(IpAddress.Ip), nameof(IpAddress.Country), nameof(IpAddress.CountryId),
            nameof(IpAddress.CreatedAt)
        ];
        var sut = new IpAddressEntityTypeConfiguration();
        var builder = EntityConfiguratationsHelper.GetEntityBuilder<IpAddress>();

        //Act
        sut.Configure(builder);

        var meta = builder.Metadata;
        var properties = meta.GetDeclaredProperties();
        var propertiesNames = properties.Select(p => p.Name).ToList();

        //Assert
        meta.Name.Should().Be(nameof(IpAddress));
        propertiesNames.Should().NotBeEmpty();
        propertiesNames.Count().Should().Be(ipAddressProperties.Count());
        propertiesNames.Should().BeEquivalentTo(propertiesNames);

        var mappedId = properties.First(p => p.Name == nameof(IpAddress.Id));
        mappedId.IsKey().Should().BeTrue();
    }
}