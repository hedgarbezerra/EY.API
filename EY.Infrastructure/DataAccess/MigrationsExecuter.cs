using EY.Infrastructure.Contracts;
using EY.Shared.Attributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EY.Infrastructure.DataAccess;

[BindInterface(typeof(IMigrationsExecuter))]
public class MigrationsExecuter : IMigrationsExecuter
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly ILogger<MigrationsExecuter> _logger;

    public MigrationsExecuter(AppDbContext context, IWebHostEnvironment hostEnvironment,
        ILogger<MigrationsExecuter> logger)
    {
        _context = context;
        _hostEnvironment = hostEnvironment;
        _logger = logger;
    }

    public void Migrate()
    {
        try
        {
            _logger.LogInformation("Migrating database...");
            var pendingMigrations = _context.Database.GetPendingMigrations().ToList();

            if (pendingMigrations is { Count: > 0 })
                _context.Database.Migrate();

            _logger.LogInformation("Database migration completed successfully.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Migrations were not applied. Reason: {MigrationFailedReason}", e.Message);
        }
    }
}