using EY.Domain.Models.Options;
using EY.Shared.Attributes;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EY.Shared.Extensions.ServiceCollection
{
    public static class CommonServiceCollectionExtensions
    {
        /// <summary>
        /// Adds every class using <see cref="BindInterfaceAttribute"/> attribute to DI container 
        /// </summary>
        /// <param name="services">Collection of services on DI container</param>
        /// <param name="assemblies">Collection of assemblies to be scanned</param>
        /// <returns>List of services</returns>
        public static IServiceCollection AddAttributeMappedServices(this IServiceCollection services, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var typesWithAttributes = assembly.GetTypes()
                    .Where(type => type.GetCustomAttributes<BindInterfaceAttribute>().Any());

                foreach (var type in typesWithAttributes)
                {
                    var attributes = type.GetCustomAttributes<BindInterfaceAttribute>();

                    foreach (var attr in attributes)
                    {
                        var serviceDescriptor = new ServiceDescriptor(attr.Interface, type, attr.Lifetime);
                        services.Add(serviceDescriptor);
                    }
                }
            }
            return services;
        }

        /// <summary>
        /// Enables distributed cache with Redis
        /// </summary>
        /// <param name="services">Collection of services on DI container</param>
        /// <returns>List of services</returns>
        /// <exception cref="ApplicationException">In the case of appsettings not being configured</exception>
        /// <remarks>Could opt to use Memory cache with services.AddDistributedMemoryCache() extension</remarks>
        public static IServiceCollection AddRedisDistributedCache(this IServiceCollection services)
        {
            var redisOptions = services.BuildServiceProvider()?.GetRequiredService<IOptions<RedisCacheOptions>>().Value;

            if (redisOptions is null)
                return services;

            services.AddSingleton(_ => new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(redisOptions.CacheExpiracyInSeconds)
            });

            services.AddOutputCache()
                .AddStackExchangeRedisOutputCache(options =>
                {
                    options.InstanceName = redisOptions.Instance;
                    options.Configuration = redisOptions.ConnectionString;
                });

            services.AddStackExchangeRedisCache(options =>
            {
                options.InstanceName = redisOptions.Instance;
                options.Configuration = redisOptions.ConnectionString;

            });

            return services;
        }
        /// <summary>
        /// Enable default resilience pipeline with timeout and 
        /// </summary>
        /// <param name="services">Collection of services on DI container</param>
        /// <param name="logger"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public static IServiceCollection AddAPIResiliencePipeline(this IServiceCollection services, ILogger logger)
        {
            var retryOptions = services.BuildServiceProvider()?.GetRequiredService<IOptions<RetryPolicyOptions>>().Value;
            if (retryOptions is null)
                return services;

            services.AddResiliencePipeline(RetryPolicyOptions.DEFAULT_PIPELINE, opt =>
            {
                //The order of the calls is the same as the order of execution. If you configure retry before timeout, retries will be made before canceling the request.
                opt
                .AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromSeconds(retryOptions.TimeOutSeconds),
                    OnTimeout = to =>
                    {
                        logger?.LogWarning("A execução para '{RequestUrl}' foi cancelada devido à demora na resposta do serviço requisitado.", to.Context.GetContextURL());
                        return default;
                    }
                })
                .AddRetry(new Polly.Retry.RetryStrategyOptions
                {
                    Delay = TimeSpan.FromSeconds(retryOptions.DelayInSeconds),
                    MaxDelay = TimeSpan.FromSeconds(retryOptions.MaxDelaySeconds),
                    MaxRetryAttempts = retryOptions.MaxRetries,
                    ShouldHandle = new PredicateBuilder().Handle<Exception>(err => !err.IsAbortedRequestException()),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = rt =>
                    {
                        int attempt = rt.AttemptNumber + 1;
                        if (attempt == retryOptions.MaxRetries)
                            logger?.LogError(rt.Outcome.Exception,
                                "Na última({AttemptNumber}) tentativa de execução da chamada para '{RequestUrl}', ainda ocorreu um erro, não sendo finalizada.",
                                attempt, rt.Context.GetContextURL());

                        return default;
                    }
                });
            });

            return services;
        }
    }
}
