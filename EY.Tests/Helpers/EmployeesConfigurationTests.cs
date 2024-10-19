using Microsoft.EntityFrameworkCore;
using NETCore.Basic.Domain.Entities;
using NETCore.Basic.Repository.DataContext.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NETCore.Basic.Tests.Repositories.Configurations
{
    internal class EmployeesConfigurationTests
    {
        private static List<string> EmployeeProperties = new List<string>()
        {
            nameof(Employee.Id),
            nameof(Employee.Name),
            nameof(Employee.Email),
            nameof(Employee.Type),
            nameof(Employee.RegisteredAt),
        };

        [Test]
        public void Configure()
        {
            var sut = new EmployeesConfiguration();
            var builder = EntityConfiguratationsHelper.GetEntityBuilder<Employee>();

            sut.Configure(builder);

            var meta = builder.Metadata;
            var properties = meta.GetDeclaredProperties();
            var propertiesNames = properties.Select(p => p.Name);

            Assert.AreEqual(nameof(Employee), meta.Name);
            Assert.IsNotEmpty(properties);
            Assert.AreEqual(EmployeeProperties.Count, properties.Count());
            Assert.AreEqual(EmployeeProperties, propertiesNames);

            var mappedId = properties.First(p => p.Name == nameof(Employee.Id));
            Assert.True(mappedId.IsKey());
        }

    }
}
