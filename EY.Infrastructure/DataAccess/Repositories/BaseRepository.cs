using System.Linq.Expressions;
using EY.Domain.Contracts;

namespace EY.Infrastructure.DataAccess.Repositories;

public abstract class BaseRepository<T> : IRepositoryBulk<T>, IRepository<T> where T : class
{
    private readonly AppDbContext _dbContext;

    public BaseRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual IQueryable<T> Get()
    {
        var found = _dbContext.Set<T>().AsQueryable();
        return found;
    }

    public virtual IQueryable<T> Get(Expression<Func<T, bool>> filter)
    {
        var found = Get().Where(filter);
        return found;
    }

    public virtual T? Get(int key)
    {
        return _dbContext.Set<T>().Find(key);
    }

    public virtual void Add(T entity)
    {
        _dbContext.Set<T>().Add(entity);
    }

    public virtual void Delete(int key)
    {
        var entity = Get(key);
        if (entity is not null)
            _dbContext.Set<T>().Remove(entity);
    }

    public virtual void Update(T entity)
    {
        _dbContext.Set<T>().Update(entity);
    }

    public virtual void BulkUpdate(params T[] entities)
    {
        _dbContext.Set<T>().UpdateRange(entities);
    }

    public virtual void BulkDelete(params T[] entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
    }
}