using EY.Business.Services;
using EY.Domain.Contracts;
using EY.Domain.Entities;
using EY.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Business.Services
{
    [TestFixture]
    public class CountriesServiceTests
    {
        private IUnitOfWork _unitOfWork;
        private IRedisCache _redisCache;
        private CountriesService _countriesService;

        [SetUp]
        public void SetUp()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _redisCache = Substitute.For<IRedisCache>();
            _countriesService = new CountriesService(_unitOfWork, _redisCache);
        }

        [Test]
        public void Add_WhenCountryExists_ReturnsFailure()
        {
            // Arrange
            var countryRepository = Substitute.For<IRepository<Country>>();
            countryRepository.Get().Returns(new List<Country> { new Country { Name = "United States", TwoLetterCode = "USA", ThreeLetterCode = "USA"  } }.AsQueryable());

            _unitOfWork.Repository<Country>().Returns(countryRepository);

            var countryInput = new CountryInput("United States", "US", "USA");

            // Act
            var result = _countriesService.Add(countryInput);

            // Assert
            result.Successful.Should().BeFalse();
            result.Errors.Should().ContainSingle($"Country with code '{countryInput.ThreeLetterCode}' already exists.");
            _redisCache.DidNotReceive().AddAsync(Arg.Any<string>(), Arg.Any<Country>());
        }

        [Test]
        public void Add_WhenCountryDoesNotExist_AddsCountryAndReturnsSuccess()
        {
            // Arrange
            var countryRepository = Substitute.For<IRepository<Country>>();
            countryRepository.Get().Returns(new List<Country>().AsQueryable());

            _unitOfWork.Repository<Country>().Returns(countryRepository);

            var countryInput = new CountryInput("Brazil", "BR", "BRA");

            // Act
            var result = _countriesService.Add(countryInput);

            // Assert
            result.Successful.Should().BeTrue();
            _unitOfWork.Received(1).Commit();
            countryRepository.Received(1).Add(Arg.Is<Country>(c => c.Name == countryInput.Name && c.ThreeLetterCode == countryInput.ThreeLetterCode));
            _redisCache.DidNotReceive().AddAsync(Arg.Any<string>(), Arg.Any<Country>());
        }

        [Test]
        public void Delete_WhenCountryDoesNotExist_ReturnsFailure()
        {
            // Arrange
            var countryRepository = Substitute.For<IRepository<Country>>();
            countryRepository.Get().Returns(new List<Country>().AsQueryable());

            _unitOfWork.Repository<Country>().Returns(countryRepository);

            // Act
            var result = _countriesService.Delete("USA");

            // Assert
            result.Successful.Should().BeFalse();
            result.Errors.Should().ContainSingle("Country with code 'USA' not found.");
        }

        [Test]
        public void Delete_WhenCountryExists_DeletesCountryAndReturnsSuccess()
        {
            // Arrange
            var countryRepository = Substitute.For<IRepository<Country>>();
            var country = new Country { Name = "United States", TwoLetterCode = "USA", ThreeLetterCode = "USA" };
            countryRepository.Get().Returns(new List<Country> { country }.AsQueryable());

            _unitOfWork.Repository<Country>().Returns(countryRepository);

            // Act
            var result = _countriesService.Delete("USA");

            // Assert
            result.Successful.Should().BeTrue();
            _unitOfWork.Received(1).Commit();
            countryRepository.Received(1).Delete(country.Id);
            _redisCache.Received(1).RemoveAsync(Arg.Is(CountriesService.CachePrefix + "USA"));
        }

        [Test]
        public async Task Get_WhenCountryExistsInCache_ReturnsCountryFromCache()
        {
            // Arrange
            var country = new Country { Name = "United States", TwoLetterCode = "USA", ThreeLetterCode = "USA" };
            _redisCache.GetAsync<Country>(CountriesService.CachePrefix + "USA", Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(country));

            // Act
            var result = await _countriesService.Get("USA");

            // Assert
            result.Successful.Should().BeTrue();
            result.Data.Should().Be(country);
        }

        [Test]
        public async Task Get_WhenCountryExistsInDatabase_ReturnsCountryFromDatabase()
        {
            // Arrange
            _redisCache.GetAsync<Country>(CountriesService.CachePrefix + "USA", Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((Country)null));

            var countryRepository = Substitute.For<IRepository<Country>>();
            var country = new Country { Name = "United States", TwoLetterCode = "USA", ThreeLetterCode = "USA" };
            countryRepository.Get().Returns(new List<Country> { country }.AsQueryable());

            _unitOfWork.Repository<Country>().Returns(countryRepository);

            // Act
            var result = await _countriesService.Get("USA");

            // Assert
            result.Successful.Should().BeTrue();
            result.Data.Should().Be(country);
            _redisCache.Received(1).AddAsync(Arg.Is(CountriesService.CachePrefix + "USA"), country);
        }

        [Test]
        public async Task Get_WhenCountryDoesNotExist_ReturnsFailure()
        {
            // Arrange
            _redisCache.GetAsync<Country>(CountriesService.CachePrefix + "USA", Arg.Any<CancellationToken>())
                .Returns(Task.FromResult((Country)null));

            var countryRepository = Substitute.For<IRepository<Country>>();
            countryRepository.Get().Returns(new List<Country>().AsQueryable());

            _unitOfWork.Repository<Country>().Returns(countryRepository);

            // Act
            var result = await _countriesService.Get("USA");

            // Assert
            result.Successful.Should().BeFalse();
            result.Errors.Should().ContainSingle("Country with code 'USA' not found.");
        }

        [Test]
        public void Update_WhenCountryDoesNotExist_ReturnsFailure()
        {
            // Arrange
            var countryRepository = Substitute.For<IRepository<Country>>();
            countryRepository.Get().Returns(new List<Country>().AsQueryable());

            _unitOfWork.Repository<Country>().Returns(countryRepository);

            var countryInput = new CountryInput("United States", "US", "USA");

            // Act
            var result = _countriesService.Update(countryInput);

            // Assert
            result.Successful.Should().BeFalse();
            result.Errors.Should().ContainSingle("Country with code 'USA' not found.");
        }

        [Test]
        public void Update_WhenCountryExists_UpdatesCountryAndReturnsSuccess()
        {
            // Arrange
            var countryRepository = Substitute.For<IRepository<Country>>();
            var country = new Country { Name = "United States", TwoLetterCode = "USA", ThreeLetterCode = "USA" };
            countryRepository.Get().Returns(new List<Country> { country }.AsQueryable());

            _unitOfWork.Repository<Country>().Returns(countryRepository);

            var countryInput = new CountryInput("United States of America", "US", "USA");

            // Act
            var result = _countriesService.Update(countryInput);

            // Assert
            result.Successful.Should().BeTrue();
            _unitOfWork.Received(1).Commit();
            country.Name.Should().Be(countryInput.Name);
            _redisCache.Received(1).AddAsync(Arg.Is(CountriesService.CachePrefix + countryInput.ThreeLetterCode), country);
        }
    }

}
