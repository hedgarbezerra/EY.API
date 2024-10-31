using EY.Business.IpAddresses;
using EY.Domain.Contracts;
using EY.Domain.Countries;
using EY.Domain.IpAddresses;
using EY.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Business.Services
{
    [TestFixture]
    public class IpAddressServiceTests
    {
        private IpAddressService _service;
        private IUnitOfWork _unitOfWork;
        private IRedisCache _redisCache;
        private IIp2CService _httpConsumer;
        private ISqlExecutor _sqlExecutor;

        [SetUp]
        public void Setup()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _redisCache = Substitute.For<IRedisCache>();
            _httpConsumer = Substitute.For<IIp2CService>();
            _sqlExecutor = Substitute.For<ISqlExecutor>();

            _service = new IpAddressService(_unitOfWork, _redisCache, _httpConsumer, _sqlExecutor);
        }

        [Test]
        public void Add_ShouldReturnSuccess_WhenIpAddressIsNew()
        {
            // Arrange
            var ipAddressInput = new IpAddressInput("192.168.1.1", "CountryName", "CN", "CNC");
            var ipRepository = Substitute.For<IRepository<IpAddress>>();
            var countriesRepository = Substitute.For<IRepository<Country>>();
            _unitOfWork.Repository<IpAddress>().Returns(ipRepository);
            _unitOfWork.Repository<Country>().Returns(countriesRepository);
            ipRepository.Get().Returns(new List<IpAddress>().AsQueryable());
            countriesRepository.Get().Returns(new List<Country>().AsQueryable());

            // Act
            var result = _service.Add(ipAddressInput);

            // Assert
            result.Should().NotBeNull();
            result.Successful.Should().BeTrue();
            ipRepository.Received(1).Add(Arg.Is<IpAddress>(ip => ip.Ip == ipAddressInput.IpAddress));
            countriesRepository.Received(1).Add(Arg.Any<Country>());
            _unitOfWork.Received(1).Commit();
            _redisCache.Received(1).AddAsync(Arg.Any<string>(), Arg.Any<IpAddress>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public void Add_ShouldReturnFailure_WhenIpAddressAlreadyExists()
        {
            // Arrange
            var ipAddressInput = new IpAddressInput("192.168.1.1", "CountryName", "CN", "CNC");
            var ipRepository = Substitute.For<IRepository<IpAddress>>();
            var countriesRepository = Substitute.For<IRepository<Country>>();
            _unitOfWork.Repository<IpAddress>().Returns(ipRepository);
            _unitOfWork.Repository<Country>().Returns(countriesRepository);
            var existingIp = new IpAddress { Ip = ipAddressInput.IpAddress };
            ipRepository.Get().Returns(new List<IpAddress> { existingIp }.AsQueryable());

            // Act
            var result = _service.Add(ipAddressInput);

            // Assert
            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Errors.Should().Contain($"IP Address '{ipAddressInput.IpAddress}' already exists on database.");
            ipRepository.DidNotReceive().Add(Arg.Any<IpAddress>());
            _unitOfWork.DidNotReceive().Commit();
            _redisCache.DidNotReceive().AddAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public void Delete_ShouldReturnSuccess_WhenIpAddressExists()
        {
            // Arrange
            var ipAddress = "192.168.1.1";
            var ipRepository = Substitute.For<IRepository<IpAddress>>();
            _unitOfWork.Repository<IpAddress>().Returns(ipRepository);
            var existingIp = new IpAddress { Ip = ipAddress, Id = 1 };
            ipRepository.Get().Returns(new List<IpAddress> { existingIp }.AsQueryable());

            // Act
            var result = _service.Delete(ipAddress);

            // Assert
            result.Should().NotBeNull();
            result.Successful.Should().BeTrue();
            ipRepository.Received(1).Delete(existingIp.Id);
            _unitOfWork.Received(1).Commit();
            _redisCache.Received(1).RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public void Delete_ShouldReturnFailure_WhenIpAddressDoesNotExist()
        {
            // Arrange
            var ipAddress = "192.168.1.1";
            var ipRepository = Substitute.For<IRepository<IpAddress>>();
            _unitOfWork.Repository<IpAddress>().Returns(ipRepository);
            ipRepository.Get().Returns(new List<IpAddress>().AsQueryable());

            // Act
            var result = _service.Delete(ipAddress);

            // Assert
            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Errors.Should().Contain($"IP address '{ipAddress}' not found.");
            ipRepository.DidNotReceive().Delete(Arg.Any<int>());
            _unitOfWork.DidNotReceive().Commit();
            _redisCache.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task Get_ShouldReturnSuccess_WhenIpAddressFoundInCache()
        {
            // Arrange
            var ipAddress = "192.168.1.1";
            var cachedIp = new IpAddress
            {
                Ip = ipAddress,
                Country = new Country
                {
                    Name = "CountryName",
                    TwoLetterCode = "CN",
                    ThreeLetterCode = "CNC"
                }
            };
            _redisCache.GetAsync<IpAddress>($"{IpAddressService.CachePrefix}{ipAddress}", CancellationToken.None)
                .Returns(Task.FromResult(cachedIp));

            // Act
            var result = await _service.Get(ipAddress);

            // Assert
            result.Should().NotBeNull();
            result.Successful.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.IpAddress.Should().Be(ipAddress);
            result.Data.CountryName.Should().Be(cachedIp.Country.Name);
            await _redisCache.Received(1).GetAsync<IpAddress>(Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task Get_ShouldReturnSuccess_WhenIpAddressFoundInDatabase()
        {
            // Arrange
            var ipAddress = "192.168.1.1";
            var dbIp = new IpAddress
            {
                Ip = ipAddress,
                Country = new Country
                {
                    Name = "CountryName",
                    TwoLetterCode = "CN",
                    ThreeLetterCode = "CNC"
                }
            };
            var ipRepository = Substitute.For<IRepository<IpAddress>>();
            _unitOfWork.Repository<IpAddress>().Returns(ipRepository);
            ipRepository.Get().Returns(new List<IpAddress> { dbIp }.AsQueryable());

            // Mock the cache to return null
            _redisCache.GetAsync<IpAddress>($"{IpAddressService.CachePrefix}{ipAddress}", CancellationToken.None)
                .Returns(Task.FromResult<IpAddress>(null));

            // Act
            var result = await _service.Get(ipAddress);

            // Assert
            result.Should().NotBeNull();
            result.Successful.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.IpAddress.Should().Be(ipAddress);
            result.Data.CountryName.Should().Be(dbIp.Country.Name);
            await _redisCache.Received(1).GetAsync<IpAddress>(Arg.Any<string>(), Arg.Any<CancellationToken>());
            await _redisCache.Received(1).AddAsync($"{IpAddressService.CachePrefix}{ipAddress}", dbIp, CancellationToken.None);
        }

        [Test]
        public async Task Get_ShouldReturnFailure_WhenIpAddressNotFoundInExternalService()
        {
            // Arrange
            var ipAddress = "192.168.1.1";
            _redisCache.GetAsync<IpAddress>($"{IpAddressService.CachePrefix}{ipAddress}", CancellationToken.None)
                .Returns(Task.FromResult<IpAddress>(null));

            var ipRepository = Substitute.For<IRepository<IpAddress>>();
            _unitOfWork.Repository<IpAddress>().Returns(ipRepository);
            ipRepository.Get().Returns(new List<IpAddress>().AsQueryable());

            // Mock the external service to return a failure
            _httpConsumer.GetIp(ipAddress, CancellationToken.None)
                .Returns(Task.FromResult(Result<Ip2CResponse>.Failure([ "Not Found"])));

            // Act
            var result = await _service.Get(ipAddress);

            // Assert
            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Errors.Should().Contain("Not Found");
        }

        [Test]
        public void Update_ShouldReturnSuccess_WhenIpAddressExists()
        {
            // Arrange
            var ipAddressInput = new IpAddressInput("192.168.1.1", "CountryName", "CN", "CNC");
            var ipRepository = Substitute.For<IRepository<IpAddress>>();
            var countriesRepository = Substitute.For<IRepository<Country>>();
            _unitOfWork.Repository<IpAddress>().Returns(ipRepository);
            _unitOfWork.Repository<Country>().Returns(countriesRepository);
            var existingIp = new IpAddress { Ip = ipAddressInput.IpAddress, Id = 1 };
            ipRepository.Get().Returns(new List<IpAddress> { existingIp }.AsQueryable());
            countriesRepository.Get().Returns(new List<Country>().AsQueryable());

            // Act
            var result = _service.Update(ipAddressInput);

            // Assert
            result.Should().NotBeNull();
            result.Successful.Should().BeTrue();
            existingIp.Country.Should().NotBeNull();
            existingIp.UpdatedAt.Should().BeAfter(DateTime.UtcNow.AddSeconds(-1));
            _unitOfWork.Received(1).Commit();
        }

        [Test]
        public void Update_ShouldReturnFailure_WhenIpAddressDoesNotExist()
        {
            // Arrange
            var ipAddressInput = new IpAddressInput("192.168.1.1", "CountryName", "CN", "CNC");
            var ipRepository = Substitute.For<IRepository<IpAddress>>();
            var countriesRepository = Substitute.For<IRepository<Country>>();
            _unitOfWork.Repository<IpAddress>().Returns(ipRepository);
            _unitOfWork.Repository<Country>().Returns(countriesRepository);
            ipRepository.Get().Returns(new List<IpAddress>().AsQueryable());

            // Act
            var result = _service.Update(ipAddressInput);

            // Assert
            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Errors.Should().Contain($"IP address '{ipAddressInput.IpAddress}' not found.");
            _unitOfWork.DidNotReceive().Commit();
        }
    }
}
