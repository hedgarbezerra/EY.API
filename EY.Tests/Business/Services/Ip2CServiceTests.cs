using EY.Business.IpAddresses;
using EY.Domain.Contracts;
using EY.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Business.Services
{
    [TestFixture]
    public class Ip2CServiceTests
    {
        private IHttpConsumer _httpConsumerMock;
        private IIp2CService _ip2CService;

        [SetUp]
        public void SetUp()
        {
            _httpConsumerMock = Substitute.For<IHttpConsumer>();
            _ip2CService = new Ip2CService(_httpConsumerMock);
        }

        [Test]
        public async Task GetIp_InvalidIpAddress_ShouldReturnFailure()
        {
            // Arrange
            string invalidIp = "invalid-ip";

            // Act
            var result = await _ip2CService.GetIp(invalidIp, CancellationToken.None);

            // Assert
            result.Successful.Should().BeFalse();
            result.Errors.Should().Contain($"IP address '{invalidIp}' is not valid.");
        }

        [Test]
        public async Task GetIp_ValidIpAddress_ReturnsSuccessfulResult()
        {
            // Arrange
            string validIp = "1.1.1.1";
            string responseData = $"1;US;USA;United States";
            _httpConsumerMock.Get<string>($"http://ip2c.org/{validIp}", Arg.Any<List<KeyValuePair<string, string>>>(), Arg.Any<CancellationToken>())
                .Returns(Result<string>.Success(responseData));

            // Act
            var result = await _ip2CService.GetIp(validIp, CancellationToken.None);

            // Assert
            result.Successful.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.CountryTwoLetterCode.Should().Be("US");
            result.Data.CountryThreeLetterCode.Should().Be("USA");
            result.Data.CountryName.Should().Be("United States");
            result.Successes.Should().Contain($"IP Address '{validIp}' response found.");
        }

        [Test]
        public async Task GetIp_ValidIpAddress_InvalidResponseFormat_ReturnsFailure()
        {
            // Arrange
            string validIp = "1.1.1.1";
            string invalidResponseData = $"1;US;USA";
            _httpConsumerMock.Get<string>($"http://ip2c.org/{validIp}", Arg.Any<List<KeyValuePair<string, string>>>(), Arg.Any<CancellationToken>())
                .Returns(Result<string>.Success(invalidResponseData));

            // Act
            var result = await _ip2CService.GetIp(validIp, CancellationToken.None);

            // Assert
            result.Successful.Should().BeFalse();
            result.Errors.Should().Contain($"IP Address '{validIp}' response not suitable.");
        }

        [Test]
        public async Task GetIp_ValidIpAddress_ResponseNotFound_ReturnsFailure()
        {
            // Arrange
            string validIp = "1.1.1.1";
            _httpConsumerMock.Get<string>($"http://ip2c.org/{validIp}", Arg.Any<List<KeyValuePair<string, string>>>(), Arg.Any<CancellationToken>())
                .Returns(Result<string>.Failure());

            // Act
            var result = await _ip2CService.GetIp(validIp, CancellationToken.None);

            // Assert
            result.Successful.Should().BeFalse();
            result.Errors.Should().Contain($"IP Address '{validIp}' not found.");
        }
    }

}
