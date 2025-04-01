using EY.Shared.Extensions;
using Microsoft.Extensions.Http.Diagnostics;
using Polly;

namespace EY.Tests.Shared.Extensions;

[TestFixture]
public class PollyExtensionsTests
{
    [SetUp]
    public void Setup()
    {
        _mockContext = ResilienceContextPool.Shared.Get();
    }

    private readonly ResiliencePropertyKey<RequestMetadata?> _requestMetadataKey = new("Extensions-RequestMetadata");

    private ResilienceContext _mockContext;

    [Test]
    public void GetContextURL_WithNullRequestMetadata_ShouldReturnEmptyString()
    {
        // Act
        var result = _mockContext.GetContextURL();

        // Assert
        result.Should().Be(string.Empty);
    }

    [Test]
    public void GetContextMetadata_WithValidRequestMetadata_ShouldReturnRequestMetadata()
    {
        // Arrange
        var requestMetadata = new RequestMetadata
        {
            RequestRoute = "https://example.com/api/resource",
            DependencyName = "NoDeps",
            MethodType = "abc"
        };
        _mockContext.Properties.Set(_requestMetadataKey, requestMetadata);

        // Act
        var result = _mockContext.GetContextMetadata();

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(requestMetadata);
        result.DependencyName.Should().Be(requestMetadata.DependencyName);
        result.RequestRoute.Should().Be(requestMetadata.RequestRoute);
        result.MethodType.Should().Be(requestMetadata.MethodType);
    }
}