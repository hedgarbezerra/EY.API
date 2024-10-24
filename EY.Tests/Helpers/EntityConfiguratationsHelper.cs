using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace EY.Tests.Helpers
{
    internal static class EntityConfiguratationsHelper
    {
        public static EntityTypeBuilder<T> GetEntityBuilder<T>() where T : class
        {
            var entityType = new EntityType(typeof(T).Name, typeof(T), new Model(), false, ConfigurationSource.Convention);

            var builder = new EntityTypeBuilder<T>(entityType);
            return builder;
        }

        public static List<string> GetEntityProperties<T>() => typeof(T).GetProperties().Select(p => p.Name).ToList();
    }
}
