using EY.Domain.Entities;
using EY.Infrastructure.DataAccess.EntitiesMaps;
using EY.Tests.Helpers;

namespace EY.Tests.Infrastructure.DataAccess.EntitiesMaps
{
    [TestFixture]
    internal class IpAddressesConfigurationTests
    {
        [Test]
        public void Configure()
        {
            var ipAddressProperties = EntityConfiguratationsHelper.GetEntityProperties<IpAddress>();
            var sut = new IpAddressEntityConfiguration();
            var builder = EntityConfiguratationsHelper.GetEntityBuilder<IpAddress>();

            sut.Configure(builder);

            var meta = builder.Metadata;
            var properties = meta.GetDeclaredProperties();
            var propertiesNames = properties.Select(p => p.Name);

            Assert.AreEqual(nameof(IpAddress), meta.Name);
            Assert.IsNotEmpty(properties);
            Assert.AreEqual(ipAddressProperties.Count, properties.Count());
            Assert.AreEqual(ipAddressProperties, propertiesNames);

            var mappedId = properties.First(p => p.Name == nameof(IpAddress.Id));
            Assert.True(mappedId.IsKey());
        }

    }
}
