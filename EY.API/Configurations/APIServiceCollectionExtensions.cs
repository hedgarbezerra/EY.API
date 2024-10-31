using Asp.Versioning;
using EY.Domain;
using EY.Domain.Models.Options;
using EY.Shared.Attributes;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;
using System.Reflection;
using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using EY.Infrastructure.DataAccess;
using EY.Shared.Extensions;
using EY.Shared;
using EY.Business;
using EY.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;

namespace EY.API.Configurations
{
    public static class APIServiceCollectionExtensions
    {
        /// <summary>
        /// Enables usage of Azure's App Configurations for managing app settings on cloud
        /// </summary>
        /// <param name="services">Collection of services on DI container</param>
        /// <param name="configurationManager">Application configurations</param>
        /// <returns>Collection of services</returns>
        /// <exception cref="ApplicationException">In the case of appsettings not being configured</exception>
        /// <remarks>Must be called before every other bindings or configurations.
        /// In the case of using local appsettings.json file, remove calling.
        /// </remarks>
        public static IServiceCollection AddAzureAppConfiguration(this IServiceCollection services, ConfigurationManager configurationManager)
        {
            var azureOptions = services.BuildServiceProvider()?.GetRequiredService<IOptions<AzureOptions>>().Value;
            if (azureOptions is null || string.IsNullOrWhiteSpace(azureOptions.AppConfigurations.ConnectionString))
                return services;

            var environment = services.BuildServiceProvider()?.GetRequiredService<IWebHostEnvironment>();

            configurationManager.AddAzureAppConfiguration(config =>
            {
                config.Connect(azureOptions.AppConfigurations.ConnectionString)
                    .ConfigureRefresh(opt =>
                    {
                        opt.Register(azureOptions.AppConfigurations.CacheSentinel, true);
                        opt.SetRefreshInterval(TimeSpan.FromSeconds(azureOptions.AppConfigurations.CacheExpiracySeconds));
                    });

                config.Select(KeyFilter.Any, LabelFilter.Null);
                config.Select(KeyFilter.Any, environment?.EnvironmentName);
            });

            services.AddAzureAppConfiguration();

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

            services.AddStackExchangeRedisCache(options =>
            {
                options.InstanceName = redisOptions.Instance;
                options.Configuration = redisOptions.ConnectionString;

            });
            return services;
        }

        /// <summary>
        /// Enables entity framework to manage databases
        /// </summary>
        /// <param name="services">Collection of services on DI container</param>
        /// <param name="configurations">Application configurations</param>
        /// <returns>List of services</returns>
        /// <exception cref="ApplicationException">In the case of appsettings not being configured</exception>
        public static IServiceCollection AddEntityFramework(this IServiceCollection services, IConfiguration configurations)
        {
            var connectionString = configurations.GetConnectionString(Constants.ConnectionStrings.SqlServer);
            if(string.IsNullOrWhiteSpace(connectionString))
                return services;

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseLazyLoadingProxies(false);
                options.EnableDetailedErrors();
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                    sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
            });

            return services;
        }

        /// <summary>
        /// Enables limiting requests to the API
        /// </summary>
        /// <param name="services">Collection of services on DI container</param>
        /// <returns>List of services</returns>
        /// <exception cref="ApplicationException">In the case of appsettings not being configured</exception>
        public static IServiceCollection AddAPIRateLimiter(this IServiceCollection services) 
        {
            var rateLimitOptions = services.BuildServiceProvider()?.GetRequiredService<IOptions<RateLimitOptions>>().Value;
            if(rateLimitOptions is null)
                return services;

            services.AddRateLimiter(opt =>
            {
                opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                opt.AddPolicy(RateLimitOptions.DEFAULT_POLICY, context =>
                    RateLimitPartition.GetFixedWindowLimiter(partitionKey: context.Connection?.RemoteIpAddress?.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = rateLimitOptions.PermitLimit,
                        Window = TimeSpan.FromSeconds(rateLimitOptions.TimeWindowSeconds),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = rateLimitOptions.QueueLimit
                    })
                );
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services">Collection of services on DI container</param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAPIVersioning(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEndpointsApiExplorer();

            double apiVersion = configuration.GetValue<double>(Constants.Options.Api.VersionKey);
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(apiVersion);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("x-api-version"), new QueryStringApiVersionReader("api-version"));
            })
            .EnableApiVersionBinding()
            .AddMvc()
            .AddApiExplorer(opt =>
            {
                opt.DefaultApiVersion = new ApiVersion(apiVersion);
                opt.GroupNameFormat = "'v'VVV";
                opt.SubstituteApiVersionInUrl = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
            });

            return services;
        }

        /// <summary>
        /// Adds every class using <see cref="BindInterfaceAttribute"/> attribute to DI container 
        /// </summary>
        /// <param name="services">Collection of services on DI container</param>
        /// <returns>List of services</returns>
        public static IServiceCollection AddAttributeMappedServices(this IServiceCollection services)
        {
            var assemblies = new[] { typeof(Program).Assembly, typeof(SharedAssemblyBinder).Assembly,
                typeof(BusinessAssemblyBinder).Assembly, typeof(InfrastructureAssemblyBinder).Assembly, };

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
    }
}
