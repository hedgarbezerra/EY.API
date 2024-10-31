using EY.Domain.Countries;
using EY.Domain.Models;
using EY.Infrastructure.DataAccess;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace EY.IntegrationTests.Dependencies
{
    [TestFixture]
    public class EntityFrameworkCountriesTests
    {
        private IntegrationsWebAppFactory _factory;
        private HttpClient _client;
        private AppDbContext _dbContext;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _factory = new IntegrationsWebAppFactory();
            await _factory.StartContainersAsync();

            _client = _factory.CreateClient();

            using var scope = _factory.Services.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            _dbContext.Database.EnsureCreated();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _factory.StopContainersAsync();
            await _factory.DisposeAsync();
            _dbContext.Database.EnsureDeleted();
        }

        [TearDown]
        public void TearDown()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Countries.ExecuteDelete();
        }


        [Test]
        public async Task AddNewCountry_ShouldThrowDbUpdateException_TwoLetterCodeInvalid()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var country = new Country { Name = "Brazil", TwoLetterCode = "BRA", ThreeLetterCode = "BRA" };
            //Act
            dbContext.Countries.Add(country);
            Func<Task> act = async () => await dbContext.SaveChangesAsync();

            //Assert
            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Test]
        public async Task AddNewCountry_ShouldThrowDbUpdateException_ThreeLetterCodeInvalid()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var country = new Country { Name = "Brazil", TwoLetterCode = "BR", ThreeLetterCode = "BRAX" };
            // Act
            dbContext.Countries.Add(country);
            Func<Task> act = async () => await dbContext.SaveChangesAsync();

            //Assert
            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Test]
        public async Task AddNewCountry_ShouldThrowDbUpdateException_NameInvalid()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var country = new Country { Name = "", TwoLetterCode = "BR", ThreeLetterCode = "BRAX" };
            // Act
            dbContext.Countries.Add(country);
            Func<Task> act = async () => await dbContext.SaveChangesAsync();

            //Assert
            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Test]
        public async Task AddCountry_ThenRetrieve_ShouldReturnSeededData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var country = new Country { Name = "Brazil", TwoLetterCode = "BR", ThreeLetterCode = "BRA" };
            //Act
            dbContext.Countries.Add(country);
            await dbContext.SaveChangesAsync();

            var dbRecord = dbContext.Countries.Find(country.Id);

            //Assert
            country.Id.Should().BeGreaterThan(0);
            dbRecord.Should().NotBeNull().And.BeEquivalentTo(country);
        }

        [Test]
        public async Task UpdateCountry_ThenRetrieve_ShouldReturnUpdatedData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            //Arrange
            await AddCountries(dbContext);

            // Act
            var dbRecord = dbContext.Countries.SingleOrDefault(country => country.ThreeLetterCode == "BRA");
            dbRecord.ThreeLetterCode = "ARB";
            dbContext.SaveChanges();

            var dbOldRecord = dbContext.Countries.SingleOrDefault(country => country.ThreeLetterCode == "BRA");
            dbOldRecord.Should().BeNull();

            dbRecord.ThreeLetterCode.Should().Be("ARB");
        }

        [Test]
        public async Task GetCountry_ThenRetrieve_ShouldReturnSeededData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            //Arrange
            await AddCountries(dbContext);

            // Act
            var dbRecord = dbContext.Countries.Where(country => country.ThreeLetterCode == "BRA");

            dbContext.Countries.Should().HaveCount(3).And.OnlyContain(c => c.TwoLetterCode == "BR" || c.TwoLetterCode == "US"|| c.TwoLetterCode == "CL");
        }

        [Test]
        public async Task RemoveCountry_ThenRetrieve_ShouldReturnSeededData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            //Arrange
            await AddCountries(dbContext);

            // Act
            int rowsRemoved = dbContext.Countries.Where(country => country.TwoLetterCode == "BR").ExecuteDelete();

            rowsRemoved.Should().Be(1);
            dbContext.Countries.Should().HaveCount(2).And.NotContain(c => c.TwoLetterCode == "BR");
        }

        private async Task AddCountries(AppDbContext dbContext)
        {
            dbContext.Countries.Add(new Country { Name = "Brazil", TwoLetterCode = "BR", ThreeLetterCode = "BRA" });
            dbContext.Countries.Add(new Country { Name = "United States", TwoLetterCode = "US", ThreeLetterCode = "USA" });
            dbContext.Countries.Add(new Country { Name = "Chile", TwoLetterCode = "CL", ThreeLetterCode = "CHL" });

            await dbContext.SaveChangesAsync();
        }
    }
}
