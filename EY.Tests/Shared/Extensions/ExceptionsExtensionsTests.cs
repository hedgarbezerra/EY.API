using EY.Shared.Extensions;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Shared.Extensions
{
    public class ExceptionsExtensionsTests
    {
        [Test]
        public void IsAbortedRequestException_WithHttpRequestException_InnerExceptionIsTaskCanceledException_ShouldReturnTrue()
        {
            // Arrange
            var innerException = new TaskCanceledException();
            var httpRequestException = new HttpRequestException("Request was aborted", innerException);

            // Act
            var result = httpRequestException.IsAbortedRequestException();

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsAbortedRequestException_WithHttpRequestException_InnerExceptionIsNotTaskCanceledException_ShouldReturnFalse()
        {
            // Arrange
            var innerException = new Exception("Some other exception");
            var httpRequestException = new HttpRequestException("Request failed", innerException);

            // Act
            var result = httpRequestException.IsAbortedRequestException();

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsAbortedRequestException_WithTimeoutRejectedException_ShouldReturnTrue()
        {
            // Arrange
            var timeoutRejectedException = new TimeoutRejectedException();

            // Act
            var result = timeoutRejectedException.IsAbortedRequestException();

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsAbortedRequestException_WithOtherException_ShouldReturnFalse()
        {
            // Arrange
            var otherException = new Exception("Some other exception");

            // Act
            var result = otherException.IsAbortedRequestException();

            // Assert
            result.Should().BeFalse();
        }
    }

}
