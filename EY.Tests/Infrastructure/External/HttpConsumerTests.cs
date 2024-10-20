//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EY.Tests.Infrastructure.External
//{
//    using System.Collections.Generic;
//    using System.Net;
//    using System.Threading;
//    using System.Threading.Tasks;
//    using EY.Domain.Contracts;
//    using EY.Domain.Models.Options;
//    using EY.Infrastructure.External;
//    using FluentAssertions;
//    using NSubstitute;
//    using NUnit.Framework;
//    using Polly.Registry;
//    using Polly;
//    using RestSharp;

//    [TestFixture]
//    public class HttpConsumerTests
//    {
//        private IHttpConsumer _httpConsumer;
//        private ResiliencePipelineProvider<string> _pipeline;
//        private RestClient _restClient;

//        [SetUp]
//        public void SetUp()
//        {
//            _pipeline = Substitute.For<ResiliencePipelineProvider<string>>();
//            _restClient = Substitute.For<RestClient>();
//            _httpConsumer = new HttpConsumer(_pipeline, _restClient);
//        }

//        [Test]
//        public async Task Get_ShouldReturnResult_WhenResponseIsSuccessful()
//        {
//            // Arrange
//            var url = "http://example.com/api/resource";
//            var response = new RestResponse<string>
//            {
//                StatusCode = HttpStatusCode.OK,
//                Content = "\"test\""
//            };
//            _restClient.ExecuteAsync<string>(Arg.Any<RestRequest>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(response));

//            // Act
//            var result = await _httpConsumer.Get<string>(url);

//            // Assert
//            result.Success.Should().BeTrue();
//            result.Value.Should().Be("test");
//        }

//        [Test]
//        public async Task Get_ShouldReturnFailure_WhenResponseIsNotSuccessful()
//        {
//            // Arrange
//            var url = "http://example.com/api/resource";
//            var response = new RestResponse<string>
//            {
//                StatusCode = HttpStatusCode.NotFound
//            };
//            _restClient.ExecuteAsync<string>(Arg.Any<RestRequest>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(response));

//            // Act
//            var result = await _httpConsumer.Get<string>(url);

//            // Assert
//            result.Success.Should().BeFalse();
//        }

//        [Test]
//        public async Task Post_ShouldReturnResult_WhenResponseIsSuccessful()
//        {
//            // Arrange
//            var url = "http://example.com/api/resource";
//            var param = new List<KeyValuePair<string, object>>
//        {
//            new KeyValuePair<string, object>("key", "value")
//        };
//            var response = new RestResponse<string>
//            {
//                StatusCode = HttpStatusCode.Created,
//                Content = "\"test\""
//            };
//            _restClient.ExecuteAsync<string>(Arg.Any<RestRequest>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(response));

//            // Act
//            var result = await _httpConsumer.Post<string>(url, param);

//            // Assert
//            result.Success.Should().BeTrue();
//            result.Value.Should().Be("test");
//        }

//        [Test]
//        public async Task Post_ShouldReturnFailure_WhenResponseIsNotSuccessful()
//        {
//            // Arrange
//            var url = "http://example.com/api/resource";
//            var param = new List<KeyValuePair<string, object>>();
//            var response = new RestResponse<string>
//            {
//                StatusCode = HttpStatusCode.BadRequest
//            };
//            _restClient.ExecuteAsync<string>(Arg.Any<RestRequest>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(response));

//            // Act
//            var result = await _httpConsumer.Post<string>(url, param);

//            // Assert
//            result.Success.Should().BeFalse();
//        }

//        [Test]
//        public async Task Put_ShouldReturnResult_WhenResponseIsSuccessful()
//        {
//            // Arrange
//            var url = "http://example.com/api/resource";
//            var param = new List<KeyValuePair<string, object>>
//        {
//            new KeyValuePair<string, object>("key", "value")
//        };
//            var response = new RestResponse<string>
//            {
//                StatusCode = HttpStatusCode.OK,
//                Content = "\"test\""
//            };
//            _restClient.ExecuteAsync<string>(Arg.Any<RestRequest>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(response));

//            // Act
//            var result = await _httpConsumer.Put<string>(url, param);

//            // Assert
//            result.Success.Should().BeTrue();
//            result.Data.Should().Be("test");
//        }

//        [Test]
//        public async Task Put_ShouldReturnFailure_WhenResponseIsNotSuccessful()
//        {
//            // Arrange
//            var url = "http://example.com/api/resource";
//            var param = new List<KeyValuePair<string, object>>();
//            var response = new RestResponse<string>
//            {
//                StatusCode = HttpStatusCode.InternalServerError
//            };
//            _restClient.ExecuteAsync<string>(Arg.Any<RestRequest>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(response));

//            // Act
//            var result = await _httpConsumer.Put<string>(url, param);

//            // Assert
//            result.Success.Should().BeFalse();
//        }

//        [Test]
//        public async Task Delete_ShouldReturnResult_WhenResponseIsSuccessful()
//        {
//            // Arrange
//            var url = "http://example.com/api/resource";
//            var response = new RestResponse<string>
//            {
//                StatusCode = HttpStatusCode.NoContent
//            };
//            _restClient.ExecuteAsync<string>(Arg.Any<RestRequest>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(response));

//            // Act
//            var result = await _httpConsumer.Delete<string>(url);

//            // Assert
//            result.Success.Should().BeTrue();
//        }

//        [Test]
//        public async Task Delete_ShouldReturnFailure_WhenResponseIsNotSuccessful()
//        {
//            // Arrange
//            var url = "http://example.com/api/resource";
//            var response = CreateResponse(HttpStatusCode.NotFound);
//            _restClient.ExecuteAsync<string>(Arg.Any<RestRequest>(), Arg.Any<CancellationToken>()).Returns(response);

//            // Act
//            var result = await _httpConsumer.Delete<string>(url);

//            // Assert
//            result.Success.Should().BeFalse();
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _restClient.Dispose();
//        }

//        private static RestResponse CreateResponse(HttpStatusCode statusCode, string content = "")
//        {
//            var request = new RestRequest();
//            var response = new RestResponse<string>(request);

//            return response;
//        }
//    }

//}
