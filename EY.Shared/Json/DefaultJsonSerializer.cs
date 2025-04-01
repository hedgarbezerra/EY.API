using EY.Domain.Contracts;
using EY.Shared.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EY.Shared.Json;

[BindInterface(typeof(IJsonHandler), ServiceLifetime.Singleton)]
[BindInterface(typeof(IJsonDesserializer), ServiceLifetime.Singleton)]
[BindInterface(typeof(IJsonSerializer), ServiceLifetime.Singleton)]
public class DefaultJsonSerializer : IJsonHandler
{
    private readonly JsonSerializerSettings _settings;

    public DefaultJsonSerializer(ILogger<DefaultJsonSerializer> logger)
    {
        _settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Error = (sender, args)
                => logger?.LogError(args?.ErrorContext?.Error,
                    "An error occurred while serializing object type '{ObjectType}'. Reason: {ErrorReason}",
                    args?.ErrorContext?.OriginalObject?.GetType()?.FullName,
                    args?.ErrorContext?.Error?.Message)
        };
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