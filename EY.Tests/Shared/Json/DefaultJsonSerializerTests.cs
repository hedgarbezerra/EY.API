using EY.Domain.Models;
using EY.Shared.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Shared.Json
{
    public class DefaultJsonSerializerTests
    {
        private ILogger<DefaultJsonSerializer> _logger;
        private DefaultJsonSerializer _jsonSerializer;

        [SetUp]
        public void SetUp()
        {
            // Initialize JsonSerializerSettings and the DefaultJsonSerializer
            _logger = Substitute.For<ILogger<DefaultJsonSerializer>>();
            _jsonSerializer = new DefaultJsonSerializer(_logger);
        }

        [Test]
        public void Serialize_Should_Throw_ArgumentNullException_When_Entity_Is_Null()
        {
            // Arrange
            object entity = null;

            // Act
            Action act = () => _jsonSerializer.Serialize(entity);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*entity*");
        }

        [Test]
        public void Serialize_Should_Return_Json_String_When_Entity_Is_Valid()
        {
            // Arrange
            var entity = new { Id = 1, Name = "Test" };
            var expectedJson = JsonConvert.SerializeObject(entity);

            // Act
            var result = _jsonSerializer.Serialize(entity);

            // Assert
            result.Should().Be(expectedJson);
        }

        [Test]
        public void Desserialize_Should_Throw_ArgumentException_When_Content_Is_NullOrWhitespace()
        {
            // Arrange
            string content = null;

            // Act
            Action act = () => _jsonSerializer.Desserialize<object>(content);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*content*");
        }

        [Test]
        public void Desserialize_Should_Return_Object_When_Valid_Json_Is_Provided()
        {
            // Arrange
            var expectedObject = new Ip2CResponse("validIP", "brazil", "BR", "BRA");
            var json = _jsonSerializer.Serialize(expectedObject);

            // Act
            var result = _jsonSerializer.Desserialize<Ip2CResponse>(json);

            // Assert
            result.Should().BeEquivalentTo(expectedObject);
        }

        [Test]
        public void Desserialize_Should_ThrowReaderException()
        {
            // Arrange
            var invalidJson = "invalid_json";

            // Act 
            Action act = () => _jsonSerializer.Desserialize<object>(invalidJson);

            //assert
            act.Should().Throw<JsonReaderException>();
        }
    }
}
