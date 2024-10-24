using EY.Domain.Contracts;
using EY.Shared.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Shared.Json
{
    [BindInterface(typeof(IJsonHandler), ServiceLifetime.Singleton)]
    [BindInterface(typeof(IJsonDesserializer), ServiceLifetime.Singleton)]
    [BindInterface(typeof(IJsonSerializer), ServiceLifetime.Singleton)]
    public class DefaultJsonSerializer : IJsonHandler
    {
        private JsonSerializerSettings _settings;

        public DefaultJsonSerializer(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        public T? Desserialize<T>(string content)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(content);

            var entity = JsonConvert.DeserializeObject<T>(content, _settings);

            return entity;
        }

        public string Serialize<T>(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var content = JsonConvert.SerializeObject(entity, _settings);

            return content;
        }
    }
}
