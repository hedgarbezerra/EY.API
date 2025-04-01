using EY.Domain.Contracts;
using EY.Shared.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace EY.Infrastructure.DataAccess;

[BindInterface(typeof(IUnitOfWork))]
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWork(AppDbContext dbContext, IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
    }

    public IRepository<T> Repository<T>() where T : class
    {
        return _serviceProvider.GetRequiredService<IRepository<T>>();
    }

    public int Commit()
    {
        return _dbContext.SaveChanges();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}