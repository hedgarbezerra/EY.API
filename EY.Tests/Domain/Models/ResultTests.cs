using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Domain.Models
{
    using EY.Domain.Models;
    using FluentAssertions;
    using NUnit.Framework;
    using System.Collections.Generic;

    [TestFixture]
    public class ResultTests
    {
        [Test]
        public void Result_Success_ShouldIndicateSuccess()
        {
            // Arrange
            var successes = new List<string> { "Operation completed successfully." };

            // Act
            var result = Result.Success(successes);

            // Assert
            result.Successful.Should().BeTrue();
            result.Errors.Should().BeEmpty();
            result.Successes.Should().BeEquivalentTo(successes);
        }

        [Test]
        public void Result_Failure_ShouldIndicateFailure()
        {
            // Arrange
            var errors = new List<string> { "An error occurred." };

            // Act
            var result = Result.Failure(errors);

            // Assert
            result.Successful.Should().BeFalse();
            result.Errors.Should().BeEquivalentTo(errors);
            result.Successes.Should().BeEmpty();
        }

        [Test]
        public void ResultGenerics_Success_ShouldReturnData()
        {
            // Arrange
            var data = "Sample data";
            var successes = new List<string> { "Data retrieved successfully." };

            // Act
            var result = Result<string>.Success(data, successes);

            // Assert
            result.Successful.Should().BeTrue();
            result.Data.Should().Be(data);
            result.Errors.Should().BeEmpty();
            result.Successes.Should().BeEquivalentTo(successes);
        }

        [Test]
        public void ResultGenerics_Failure_ShouldReturnNullData()
        {
            // Arrange
            var errors = new List<string> { "Failed to retrieve data." };

            // Act
            var result = Result<string>.Failure(errors);

            // Assert
            result.Successful.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Errors.Should().BeEquivalentTo(errors);
            result.Successes.Should().BeEmpty();
        }

        [Test]
        public void ResultGenerics_Success_WithNoSuccessMessages_ShouldReturnEmptySuccesses()
        {
            // Arrange
            var data = "Sample data";

            // Act
            var result = Result<string>.Success(data);

            // Assert
            result.Successful.Should().BeTrue();
            result.Data.Should().Be(data);
            result.Errors.Should().BeEmpty();
            result.Successes.Should().BeEmpty();
        }

        [Test]
        public void Result_Failure_WithNoErrors_ShouldReturnEmptyErrors()
        {
            // Arrange

            // Act
            var result = Result.Failure();

            // Assert
            result.Successful.Should().BeFalse();
            result.Errors.Should().BeEmpty();
            result.Successes.Should().BeEmpty();
        }
    }

}
