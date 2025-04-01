using EY.Shared.Extensions;

namespace EY.Tests.Shared.Extensions;

[TestFixture]
public class StringExtensionsTests
{
    [Test]
    [TestCase("192.168.1.1")]
    [TestCase("255.255.255.255")]
    [TestCase("0.0.0.0")]
    [TestCase("::1")]
    [TestCase("2001:0db8:85a3:0000:0000:8a2e:0370:7334")]
    public void IsValidIpAddress_ValidIpAddresses_ShouldReturnTrue(string validIp)
    {
        // Act
        var result = validIp.IsValidIpAddress();

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    [TestCase("192.168.1.256")]
    [TestCase("256.256.256.256")]
    [TestCase("abc.def.ghi.jkl")]
    [TestCase("::g")]
    [TestCase("2001:0db8:85a3:0000:0000:8a2e:0370:zzzz")]
    [TestCase("invalid ip address")]
    [TestCase("home")]
    public void IsValidIpAddress_InvalidIpAddresses_ShouldReturnFalse(string invalidIp)
    {
        // Act
        var result = invalidIp.IsValidIpAddress();

        // Assert
        result.Should().BeFalse();
    }
}