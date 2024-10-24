using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Contracts
{
    /// <summary>
    /// Provides a method to convert JSON strings into objects of a specified type.
    /// </summary>
    public interface IJsonDesserializer
    {
        /// <summary>
        /// Deserializes a JSON string into an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize. Must be a reference type.</typeparam>
        /// <param name="content">The JSON string to deserialize.</param>
        /// <returns>The deserialized object of type <typeparamref name="T"/>, or null if the deserialization fails.</returns>
        public T? Desserialize<T>(string content);
    }

    /// <summary>
    /// Provides a method to convert objects into JSON strings.
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// Serializes an object into a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize. Must be a reference type.</typeparam>
        /// <param name="entity">The object to serialize.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public string Serialize<T>(T entity);
    }

    /// <summary>
    /// A combined interface that includes both JSON serialization and deserialization capabilities.
    /// Inherits from both <see cref="IJsonDesserializer"/> and <see cref="IJsonSerializer"/>.
    /// </summary>
    public interface IJsonHandler : IJsonDesserializer, IJsonSerializer
    {
    }

}
