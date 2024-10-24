using EY.Shared.Extensions;
using Microsoft.Extensions.Http.Diagnostics;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Shared.Extensions
{
    [TestFixture]
    public class PollyExtensionsTests
    {
        private readonly ResiliencePropertyKey<RequestMetadata?> _requestMetadataKey = new ResiliencePropertyKey<RequestMetadata>("Extensions-RequestMetadata");

        private ResilienceContext _mockContext;

        [SetUp]
        public void Setup()
        {
            _mockContext = ResilienceContextPool.Shared.Get();

        }

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

}
