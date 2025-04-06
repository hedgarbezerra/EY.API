using EY.Infrastructure.Contracts;
using EY.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace EY.IntegrationTests;

public abstract class IntegrationTestBase
{
    protected IntegrationsWebAppFactory Factory => GlobalTestSetup.Factory;
    protected HttpClient Client => GlobalTestSetup.Client;
    protected AppDbContext DbContext => GlobalTestSetup.DbContext;
    protected IMigrationsExecuter MigrationsExecuter => GlobalTestSetup.MigrationsExecuter;

    protected async Task ResetDatabase()
    {
        await DbContext.Database.ExecuteSqlAsync($"DELETE FROM [dbo].[IPAddresses]");
        await DbContext.Database.ExecuteSqlAsync($"DELETE FROM [dbo].[Countries]");
        await DbContext.SaveChangesAsync();
    }

    protected async Task SeedTestData<T>(T entity) where T : class
    {
        DbContext.Set<T>().Add(entity);
        await DbContext.SaveChangesAsync();
    }

    protected async Task SeedTestData<T>(params T[] entities) where T : class
    {
        DbContext.Set<T>().AddRange(entities);
        await DbContext.SaveChangesAsync();
    }

    protected async Task ClearTable<T>() where T : class
    {
        await DbContext.Set<T>().ExecuteDeleteAsync();
    }
}