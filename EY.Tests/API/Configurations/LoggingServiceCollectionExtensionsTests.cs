using EY.API;
using EY.API.Configurations;
using EY.Domain.Models.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.API.Configurations
{
    [TestFixture]
    public class LoggingServiceCollectionExtensionsTests
    {
        private IServiceCollection _services;

        [SetUp]
        public void SetUp()
        {
            _services = new ServiceCollection();
        }

        [Test]
        public void AddSerilogLogging_ShouldAddSerilogToServices()
        {
            // Arrange
            var otlpOptions = Options.Create(new OpenTelemetryOptions
            {
                Endpoint = "http://example.com",
                Key = "sample-key",
                Source = "TestService"
            });

            var environment = Substitute.For<IWebHostEnvironment>();
            environment.EnvironmentName.Returns("Development");

            _services.AddSingleton(otlpOptions);
            _services.AddSingleton(environment);

            // Act
            _services.AddSerilogLogging();

            // Assert
            _services.Should().ContainSingle(descriptor => descriptor.ServiceType == typeof(ILoggerFactory));
            _services.Should().ContainSingle(descriptor => descriptor.ServiceType == typeof(Serilog.IDiagnosticContext));
        }

        [Test]
        public void AddOtlpLogging_ShouldAddOpenTelemetryToServices()
        {
            // Arrange
            var otlpOptions = Options.Create(new OpenTelemetryOptions
            {
                Endpoint = "http://example.com",
                Key = "sample-key",
                Source = "TestService"
            });

            var environment = Substitute.For<IWebHostEnvironment>();
            environment.EnvironmentName.Returns("Development");

            _services.AddSingleton(otlpOptions);
            _services.AddSingleton(environment);

            // Act
            _services.AddOtlpLogging();

            // Assert
            var provider = _services.BuildServiceProvider();

            var openTelemetryLogger = provider.GetService<ILogger<Program>>();
            openTelemetryLogger.Should().NotBeNull();
            _services.Should().ContainSingle(descriptor => descriptor.ServiceType == typeof(ILoggerProvider));
        }

        [Test]
        public void AddOtlpLogging_ShouldThrowExceptionWhenOptionsNotConfigured()
        {
            // Arrange
            var environment = Substitute.For<IWebHostEnvironment>();
            environment.EnvironmentName.Returns("Development");

            _services.AddSingleton(environment);

            // Act
            Action act = () => _services.AddOtlpLogging();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }
    }

}
