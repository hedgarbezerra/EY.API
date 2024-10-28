using EY.Domain.Entities;
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
    public class EntityFrameworkTests
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
            await AddCountries(_dbContext);
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
            dbContext.IpAddresses.ExecuteDelete();
        }

        [Test]
        public async Task AddNewIpAddress_EmptyIpAddress_ShouldThrowDbUpdateException()
        {
            //Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var ipAddress = new IpAddress { CountryId = 1 };
            // Act
            dbContext.IpAddresses.Add(ipAddress);
            Func<Task> act = async () => await dbContext.SaveChangesAsync();

            //Assert
            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Test]
        public async Task AddNewIpAddress_IpAddressTooBig_ShouldThrowDbUpdateException()
        {
            //Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var ipAddress = new IpAddress { Ip = "34.343.43.43.434.3.41235.56", CountryId = 1 };
            // Act
            dbContext.IpAddresses.Add(ipAddress);
            Func<Task> act = async () => await dbContext.SaveChangesAsync();
            
            //Assert
            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Test]
        public async Task AddNewIpAddress_ValidIpAddress_ShouldAddRecord()
        {
            //Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var bra = dbContext.Countries.SingleOrDefault(c => c.TwoLetterCode == "BR");

            //Act
            var ipAddress = new IpAddress { Ip = IPAddressGenerator.Generate(), Country = bra };
            dbContext.IpAddresses.Add(ipAddress);
            await dbContext.SaveChangesAsync();

            //Assert
            ipAddress.Id.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task Get_SingleByCountry_ShouldReturnSingleRecord()
        {
            //Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await AddIpAddresses(dbContext);

            // Act
            var ips = dbContext.IpAddresses.Where(ip => ip.Country.TwoLetterCode == "BR");

            //Assert
            ips.Should().HaveCount(1);
        }

        [Test]
        public async Task Delete_DeletesBrazilianIps_ShouldHaveOneIp()
        {
            //Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await AddIpAddresses(dbContext);

            //Act
            await dbContext.IpAddresses.Where(ip => ip.Country.TwoLetterCode == "BR").ExecuteDeleteAsync();
            await dbContext.SaveChangesAsync();

            //Assert
            dbContext.IpAddresses.Should().HaveCount(1)
                .And.NotContain(i => i.Country.TwoLetterCode == "BRA");
        }

        private async Task AddCountries(AppDbContext dbContext)
        {
            dbContext.Countries.Add(new Country { Name = "Brazil", TwoLetterCode = "BR", ThreeLetterCode = "BRA" });
            dbContext.Countries.Add(new Country { Name = "United States", TwoLetterCode = "US", ThreeLetterCode = "USA" });

            await dbContext.SaveChangesAsync();
        }

        private async Task AddIpAddresses(AppDbContext dbContext)
        {
            dbContext.IpAddresses.AddRange(new IpAddress { Ip = IPAddressGenerator.Generate(), CountryId = 2 },
                new IpAddress { Ip = IPAddressGenerator.Generate(), CountryId = 1 });
            await dbContext.SaveChangesAsync();
        }
    }
}
