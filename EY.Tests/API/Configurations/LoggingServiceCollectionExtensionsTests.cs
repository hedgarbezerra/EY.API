﻿using EY.API;
using EY.API.Configurations;
using EY.Domain.Models.Options;
using EY.Shared.Extensions.ServiceCollection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
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
            var otlpOptions = GetOpenTelemetryOptions();
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
            var otlpOptions = GetOpenTelemetryOptions();
            var environment = Substitute.For<IWebHostEnvironment>();
            environment.EnvironmentName.Returns("Development");

            _services.AddSingleton(otlpOptions);
            _services.AddSingleton(environment);

            // Act
            _services.AddDistributedOpenTelemetry();

            // Assert
            _services.Should().Contain(descriptor => descriptor.ServiceType == typeof(ILoggerFactory));
            _services.Should().Contain(descriptor => descriptor.ServiceType == typeof(Serilog.IDiagnosticContext));    
            _services.Should().Contain(descriptor => descriptor.ServiceType == typeof(MeterProvider));
            _services.Should().Contain(descriptor => descriptor.ServiceType == typeof(TracerProvider));
        }

        [Test]
        public void AddOtlpLogging_ShouldThrowExceptionWhenOptionsNotConfigured()
        {
            // Arrange
            var environment = Substitute.For<IWebHostEnvironment>();
            environment.EnvironmentName.Returns("Development");

            _services.AddSingleton(environment);

            // Act
            Action act = () => _services.AddDistributedOpenTelemetry();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        private static IOptions<OpenTelemetryOptions> GetOpenTelemetryOptions() => Options.Create(new OpenTelemetryOptions
        {
            Source = "EY",
            Jaeger = new JaegerOpenTelemetryOptions { Endpoint = "" },
            Seq = new SeqOpenTelemetryOptions
            {
                Endpoint = "http://example.com",
                Key = "sample-key"
            }
        });
    }

}
