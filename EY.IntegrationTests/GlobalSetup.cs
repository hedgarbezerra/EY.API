using EY.Infrastructure.Contracts;
using EY.Infrastructure.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace EY.IntegrationTests;

[SetUpFixture]
public class GlobalTestSetup
{
    public static IntegrationsWebAppFactory Factory { get; private set; }
    public static HttpClient Client { get; private set; }
    public static AppDbContext DbContext { get; private set; }
    public static IMigrationsExecuter MigrationsExecuter { get; private set; }

    [OneTimeSetUp]
    public void Setup()
    {
        Factory = new IntegrationsWebAppFactory();
        Factory.StartContainersAsync().Wait();
        Client = Factory.CreateClient();

        var scope = Factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        MigrationsExecuter = scope.ServiceProvider.GetRequiredService<IMigrationsExecuter>();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        Factory?.Dispose();
        DbContext?.Dispose();
        Client?.Dispose();
    }
}