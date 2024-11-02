using EY.Domain.Countries;
using EY.Domain.Models;
using EY.Infrastructure.DataAccess;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.IntegrationTests.API
{
    [TestFixture]
    public class CountriesEndpointsTests
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
            _dbContext.Dispose();
            _client.Dispose();
        }

        [Test]
        public async Task SeedDatabase_ThenRetrieve_ShouldReturnSeededData()
        {
            //Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var country = new Country { Name = "Brazil", TwoLetterCode = "BR", ThreeLetterCode = "BRA" };
            dbContext.Countries.Add(country);
            await dbContext.SaveChangesAsync();

            // Act - Test retrieval from seeded database
            var response = await _client.GetAsync("/api/countries/BRA");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            result.Should().NotBeEmpty();

            Result<Country> convertedResult = JsonConvert.DeserializeObject<Result<Country>>(result);
            country.Should().NotBeNull().And.BeEquivalentTo(country);
        }
    }
}
