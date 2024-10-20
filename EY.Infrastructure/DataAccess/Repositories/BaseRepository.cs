using EY.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Polly;
using System;
using System.Linq.Expressions;

namespace EY.Infrastructure.DataAccess.Repositories
{
    public class BaseRepository<T> : ISqlExecutor<T>, IRepository<T> where T : class
    {
        private readonly AppDbContext _dbContext;

        public BaseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(T obj)
        {
            _dbContext.Set<T>().Add(obj);
        }

        public void Delete(int key)
        {
            T? entity = Get(key);
            if (entity is not null)
                _dbContext.Set<T>().Remove(entity);
        }

        public IQueryable<T> Get() => _dbContext.Set<T>().AsQueryable();

        public IQueryable<T> Get(Expression<Func<T, bool>> filter) => Get().Where(filter);

        public T? Get(int key) => _dbContext.Set<T>().Find(key);

        public void Update(T obj)
        {
            _dbContext.Set<T>().Update(obj);
        }

        public IQueryable<T> Query(FormattableString query)=> _dbContext.Database.SqlQuery<T>(query);
        public IQueryable<T> Query(string sql, params object[] parameters)=> _dbContext.Database.SqlQueryRaw<T>(sql, parameters);
        public int Execute(FormattableString query) => _dbContext.Database.ExecuteSql(query);
    }
}
