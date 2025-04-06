using EY.Domain.Countries;
using EY.Domain.Models;
using FluentAssertions;
using Newtonsoft.Json;

namespace EY.IntegrationTests.API;

[TestFixture]
public class CountriesEndpointsTests : IntegrationTestBase
{
    [Test]
    public async Task SeedDatabase_ThenRetrieve_ShouldReturnSeededData()
    {
        //Arrange
        var country = new Country { Name = "Brazil", TwoLetterCode = "BR", ThreeLetterCode = "BRA" };
        DbContext.Countries.Add(country);
        await DbContext.SaveChangesAsync();

        // Act - Test retrieval from seeded database
        var response = await Client.GetAsync("/api/countries/BRA");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        result.Should().NotBeEmpty();

        Result<Country> convertedResult = JsonConvert.DeserializeObject<Result<Country>>(result);
        country.Should().NotBeNull().And.BeEquivalentTo(country);
    }
}