using EY.Domain.Countries;
using EY.Domain.IpAddresses;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EY.IntegrationTests.Dependencies;

[TestFixture]
public class EntityFrameworkTests : IntegrationTestBase
{
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await ResetDatabase();
        await AddCountries();
    }

    [TearDown]
    public async Task TearDown()
    {
        await ClearTable<IpAddress>();
    }

    [Test]
    public async Task AddNewIpAddress_EmptyIpAddress_ShouldThrowDbUpdateException()
    {
        //Arrange
        var ipAddress = new IpAddress { Ip = null!, CountryId = 1 };
        // Act
        DbContext.IpAddresses.Add(ipAddress);
        Func<Task> act = async () => await DbContext.SaveChangesAsync();

        //Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Test]
    public async Task AddNewIpAddress_IpAddressTooBig_ShouldThrowDbUpdateException()
    {
        //Arrange
        var ipAddress = new IpAddress { Ip = "34.343.43.43.434.3121212.41235.56", CountryId = 1 };
        // Act
        DbContext.IpAddresses.Add(ipAddress);
        Func<Task> act = async () => await DbContext.SaveChangesAsync();

        //Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Test]
    public async Task AddNewIpAddress_ValidIpAddress_ShouldAddRecord()
    {
        //Arrange
        var bra = DbContext.Countries.SingleOrDefault(c => c.TwoLetterCode == "BR");

        //Act
        var ipAddress = new IpAddress { Ip = IPAddressGenerator.Generate(), Country = bra };
        DbContext.IpAddresses.Add(ipAddress);
        await DbContext.SaveChangesAsync();

        //Assert
        ipAddress.Id.Should().BeGreaterThan(0);
    }

    [Test]
    public async Task Get_SingleByCountry_ShouldReturnSingleRecord()
    {
        //Arrange
        await AddIpAddresses();

        // Act
        var ips = DbContext.IpAddresses.Where(ip => ip.Country.TwoLetterCode == "BR");

        //Assert
        ips.Should().HaveCount(1);
    }

    [Test]
    public async Task Delete_DeletesBrazilianIps_ShouldHaveOneIp()
    {
        //Arrange
        await AddIpAddresses();

        //Act
        await DbContext.IpAddresses.Where(ip => ip.Country.TwoLetterCode == "BR").ExecuteDeleteAsync();
        await DbContext.SaveChangesAsync();

        //Assert
        DbContext.IpAddresses.Should().HaveCount(1)
            .And.NotContain(i => i.Country.TwoLetterCode == "BRA");
    }

    private async Task AddCountries()
    {
        DbContext.Countries.AddRange(
            new Country { Name = "Brazil", TwoLetterCode = "BR", ThreeLetterCode = "BRA" },
            new Country { Name = "United States", TwoLetterCode = "US", ThreeLetterCode = "USA" });

        await DbContext.SaveChangesAsync();
    }

    private async Task AddIpAddresses()
    {
        DbContext.IpAddresses.AddRange(
            new IpAddress { Ip = IPAddressGenerator.Generate(), CountryId = 2 },
            new IpAddress { Ip = IPAddressGenerator.Generate(), CountryId = 1 });
        await DbContext.SaveChangesAsync();
    }
}