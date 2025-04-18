﻿using EY.Domain.Countries;
using EY.Domain.IpAddresses;
using EY.Domain.Models;
using EY.Infrastructure.DataAccess;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EY.IntegrationTests.API;

[TestFixture]
public class IpAddressesEndpointsTests : IntegrationTestBase
{
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await AddIpAddresses(DbContext);
    }

    [Test]
    public async Task GetIp_DoesNotExistInDatabase_FetchFromIp2C()
    {
        //Arrange
        var expectedIp = "1";

        //Act
        var response = await Client.GetAsync($"/api/ipaddresses/{expectedIp}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        result.Should().NotBeEmpty();

        Result<Ip2CResponse> convertedResult = JsonConvert.DeserializeObject<Result<Ip2CResponse>>(result);
    }

    [Test]
    public async Task GetIp_FoundInDatabase_ReturnRecordAndAddToCache()
    {
        //Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var expectedIp = dbContext.IpAddresses.First();

        //Act
        var response = await Client.GetAsync($"/api/ipaddresses/{expectedIp.Ip}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        result.Should().NotBeEmpty();

        Result<Ip2CResponse> convertedResult = JsonConvert.DeserializeObject<Result<Ip2CResponse>>(result);
    }

    private async Task AddCountries(AppDbContext dbContext)
    {
        dbContext.Countries.Add(new Country { Name = "Brazil", TwoLetterCode = "BR", ThreeLetterCode = "BRA" });

        await dbContext.SaveChangesAsync();
    }

    private async Task AddIpAddresses(AppDbContext dbContext)
    {
        await AddCountries(dbContext);

        dbContext.IpAddresses.AddRange(new IpAddress { Ip = "129.01.01", CountryId = 1 },
            new IpAddress { Ip = IPAddressGenerator.Generate(), CountryId = 1 },
            new IpAddress { Ip = IPAddressGenerator.Generate(), CountryId = 1 },
            new IpAddress { Ip = IPAddressGenerator.Generate(), CountryId = 1 },
            new IpAddress { Ip = IPAddressGenerator.Generate(), CountryId = 1 });

        await dbContext.SaveChangesAsync();
    }
}