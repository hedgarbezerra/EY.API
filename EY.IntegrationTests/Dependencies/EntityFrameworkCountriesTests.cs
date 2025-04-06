using EY.Domain.Countries;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EY.IntegrationTests.Dependencies;

[TestFixture]
public class EntityFrameworkCountriesTests : IntegrationTestBase
{
    [Test]
    public async Task AddNewCountry_ShouldThrowDbUpdateException_TwoLetterCodeInvalid()
    {
        var country = new Country { Name = "Brazil", TwoLetterCode = "BRA", ThreeLetterCode = "BRA" };
        //Act
        DbContext.Countries.Add(country);
        Func<Task> act = async () => await DbContext.SaveChangesAsync();

        //Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Test]
    public async Task AddNewCountry_ShouldThrowDbUpdateException_ThreeLetterCodeInvalid()
    {
        var country = new Country { Name = "Brazil", TwoLetterCode = "BR", ThreeLetterCode = "BRAX" };
        // Act
        DbContext.Countries.Add(country);
        Func<Task> act = async () => await DbContext.SaveChangesAsync();

        //Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Test]
    public async Task AddNewCountry_ShouldThrowDbUpdateException_NameInvalid()
    {
        var country = new Country { Name = null!, TwoLetterCode = "BR", ThreeLetterCode = "BRA" };
        // Act
        DbContext.Countries.Add(country);
        Func<Task> act = async () => await DbContext.SaveChangesAsync();

        //Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Test]
    public async Task AddCountry_ThenRetrieve_ShouldReturnSeededData()
    {
        var country = new Country { Name = "Brazil", TwoLetterCode = "BR", ThreeLetterCode = "BRA" };
        //Act
        DbContext.Countries.Add(country);
        await DbContext.SaveChangesAsync();

        var dbRecord = DbContext.Countries.Find(country.Id);

        //Assert
        country.Id.Should().BeGreaterThan(0);
        dbRecord.Should().NotBeNull().And.BeEquivalentTo(country);
    }

    [Test]
    public async Task UpdateCountry_ThenRetrieve_ShouldReturnUpdatedData()
    {
        //Arrange
        await AddCountries();

        // Act
        var dbRecord = DbContext.Countries.SingleOrDefault(country => country.ThreeLetterCode == "BRA");
        dbRecord.ThreeLetterCode = "ARB";
        DbContext.SaveChanges();

        var dbOldRecord = DbContext.Countries.SingleOrDefault(country => country.ThreeLetterCode == "BRA");
        dbOldRecord.Should().BeNull();

        dbRecord.ThreeLetterCode.Should().Be("ARB");
    }

    [Test]
    public async Task GetCountry_ThenRetrieve_ShouldReturnSeededData()
    {
        //Arrange
        await AddCountries();

        // Act
        var dbRecord = DbContext.Countries.Where(country => country.ThreeLetterCode == "BRA");

        DbContext.Countries.Should().HaveCount(3).And.OnlyContain(c =>
            c.TwoLetterCode == "BR" || c.TwoLetterCode == "US" || c.TwoLetterCode == "CL");
    }

    [Test]
    public async Task RemoveCountry_ThenRetrieve_ShouldReturnSeededData()
    {
        //Arrange
        await AddCountries();

        // Act
        var rowsRemoved = DbContext.Countries.Where(country => country.TwoLetterCode == "BR").ExecuteDelete();

        rowsRemoved.Should().Be(1);
        DbContext.Countries.Should().HaveCount(2).And.NotContain(c => c.TwoLetterCode == "BR");
    }

    private async Task AddCountries()
    {
        DbContext.Countries.AddRange(
            new Country { Name = "Brazil", TwoLetterCode = "BR", ThreeLetterCode = "BRA" },
            new Country { Name = "United States", TwoLetterCode = "US", ThreeLetterCode = "USA" },
            new Country { Name = "Chile", TwoLetterCode = "CL", ThreeLetterCode = "CHL" });

        await DbContext.SaveChangesAsync();
    }
}