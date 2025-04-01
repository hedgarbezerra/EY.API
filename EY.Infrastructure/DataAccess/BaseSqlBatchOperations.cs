using System.Linq.Expressions;
using EY.Shared.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace EY.Infrastructure.DataAccess;

public class BaseSqlBatchOperations<T> : ISqlBatchOperation<T> where T : class
{
    private readonly AppDbContext _dbContext;

    public BaseSqlBatchOperations(AppDbContext appDbContext)
    {
        _dbContext = appDbContext;
    }

    public int ExecuteDelete(Expression<Func<T, bool>> query)
    {
        var entities = _dbContext.Set<T>().Where(query);

        var updatedRows = entities.ExecuteDelete();

        return updatedRows;
    }

    public int ExecuteUpdate(Expression<Func<T, bool>> query,
        Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls)
    {
        var entities = _dbContext.Set<T>().Where(query);

        var updatedRows = entities.ExecuteUpdate(setPropertyCalls);

        return updatedRows;
    }
}