using EY.Domain;
using EY.Domain.Contracts;
using EY.Domain.Entities;
using System.Collections.Concurrent;

namespace EY.API.BackgroundServices
{
    public class IpAddressUpdaterTask : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly int _repeatEveryMinutes;

        private const int BatchSize = 100;
        public IpAddressUpdaterTask(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _repeatEveryMinutes = configuration.GetValue<int>(Constants.Options.Tasks.IpAddressUpdater_RepeatInMinutesKey);
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(_repeatEveryMinutes));
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    ILogger<IpAddressUpdaterTask> logger = scope.ServiceProvider.GetRequiredService<ILogger<IpAddressUpdaterTask>>();
                    IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    try
                    {
                        logger.LogInformation($"Running IP Addresses update task at {DateTime.UtcNow.ToShortTimeString()}");
                        var repository = unitOfWork.Repository<IpAddress>();
                        var ipAddresses = repository.Get();

                        for (int i = 0; i < ipAddresses.Count(); i += BatchSize)
                        {
                            var batch = ipAddresses.Skip(i).Take(BatchSize).ToList();
                            logger.LogInformation($"Processing batch of {batch.Count} IP addresses.");

                            var tasks = batch.Select(ip => Update(ip, stoppingToken)).ToList();
                            await Task.WhenAll(tasks);
                        }

                        // Remover? Fazer lógica para executar várias chamadas em simultâneo
                        var ConcurrentBag = new ConcurrentBag<IpAddress>();
                        double threadsAvailable = Environment.ProcessorCount / 2;
                        var parallelOptions = new ParallelOptions
                        {
                            CancellationToken = stoppingToken,
                            MaxDegreeOfParallelism = Math.Max(1, (int)Math.Round(threadsAvailable, MidpointRounding.ToEven)) // Set the maximum number of concurrent tasks
                        };

                        Parallel.ForEachAsync(ipAddresses, parallelOptions, async (ip, ct) =>
                        {
                            //TODO: Request do serviço externo e atualizar banco com ExecuteUpdate + remover do cache
                        });
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ex.Message);
                    }
                    finally
                    {
                        logger.LogInformation($"Done executing IP Addresses update task at {DateTime.UtcNow.ToShortTimeString()}");
                    }
                }
            }
        }

        private async Task Update(IpAddress ip, CancellationToken cancellationToken)
        {
            //TODO: Toda lógica
        }
    }
}
