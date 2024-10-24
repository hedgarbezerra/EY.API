using EY.Domain.Models;
using EY.Shared.Extensions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Shared.Extensions
{
    [TestFixture]
    public class ResultExtensionsTests
    {
        [Test]
        public void FromRestResponse_NullResponse_ShouldReturnFailure()
        {
            // Arrange
            RestResponse<string> nullResponse = null;

            // Act
            var result = nullResponse.FromRestResponse();

            // Assert
            result.Successful.Should().BeFalse();
            result.Errors.Should().ContainSingle().Which.Should().Be("No data available.");
            result.Successes.Should().BeEmpty();
        }

        [Test]
        public void FromRestResponseWithString_SuccessfulResponse_ShouldReturnSuccess()
        {
            // Arrange
            var response = new RestResponse<string>(new RestRequest())
            {
                ResponseStatus = ResponseStatus.Completed,
                IsSuccessStatusCode = true,
                Data = "Valid Data",
                Content = "Content"
            };

            // Act
            var result = response.FromRestResponse();

            // Assert
            result.Successful.Should().BeTrue();
            result.Data.Should().Be("Content");
            result.Errors.Should().BeEmpty();
            result.Successes.Should().BeEmpty();
        }
        [Test]
        public void FromRestResponse_SuccessfulResponse_ShouldReturnSuccess()
        {
            // Arrange
            var responseData = new Ip2CResponse("123.09213.2192", "Canada", "CA", "CAD");
            var response = new RestResponse<Ip2CResponse>(new RestRequest())
            {
                ResponseStatus = ResponseStatus.Completed,
                IsSuccessStatusCode = true,
                Data = responseData,
                Content = "Content"
            };

            // Act
            var result = response.FromRestResponse();

            // Assert
            result.Successful.Should().BeTrue();
            result.Data.Should().Be(responseData);
            result.Errors.Should().BeEmpty();
            result.Successes.Should().BeEmpty();
        }

        [Test]
        public void FromRestResponse_UnsuccessfulResponse_ShouldReturnFailure()
        {
            // Arrange
            var response = new RestResponse<string>(new RestRequest())
            {
                IsSuccessStatusCode = false,
                ErrorMessage = "Error occurred."
            };

            // Act
            var result = response.FromRestResponse();

            // Assert
            result.Successful.Should().BeFalse();
            result.Errors.Should().ContainSingle().Which.Should().Be("Error occurred.");
            result.Successes.Should().BeEmpty();
        }
    }
}
