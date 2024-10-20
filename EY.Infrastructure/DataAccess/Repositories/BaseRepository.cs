using EY.Domain.Contracts;
using EY.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using OpenTelemetry.Resources;
using Polly;
using System;
using System.Linq.Expressions;

namespace EY.Infrastructure.DataAccess.Repositories
{
    public class BaseRepository<T> : ISqlExecutor<T>, IRepositoryBulk<T>, IRepository<T> where T : class
    {
        private readonly AppDbContext _dbContext;

        public BaseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IQueryable<T> Get(bool includeRelated = false)
        {
            var found = _dbContext.Set<T>().AsQueryable();
            if(includeRelated)
                IncludeAllRelatedEntities(found);

            return found;
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> filter, bool includeRelated = false)
        {
            var found = Get().Where(filter);
            if (includeRelated)
                IncludeAllRelatedEntities(found);

            return found;
        }

        public T? Get(int key) => _dbContext.Set<T>().Find(key);

        public void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        public void Delete(int key)
        {
            T? entity = Get(key);
            if (entity is not null)
                _dbContext.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }


        public IQueryable<T> Query(FormattableString query)=> _dbContext.Database.SqlQuery<T>(query);
        public IQueryable<T> Query(string sql, params object[] parameters)=> _dbContext.Database.SqlQueryRaw<T>(sql, parameters);
        public int Execute(FormattableString query) => _dbContext.Database.ExecuteSql(query);


        public void BulkUpdate(params T[] entities) => _dbContext.Set<T>().UpdateRange(entities);
        public void BulkDelete(params T[] entities) => _dbContext.Set<T>().RemoveRange(entities);


        protected IQueryable<T> IncludeAllRelatedEntities(IQueryable<T> query)
        {
            var entityType = _dbContext.Model.FindEntityType(typeof(T));
            if(entityType is null) 
                return query;

            var navigations = entityType.GetNavigations().Select(n => n.Name);

            foreach (var navigation in navigations)
            {
                query = query.Include(navigation);
            }

            return query;
        }

    }
}
