using EY.Domain.Contracts;
using System.Linq.Expressions;

namespace EY.Infrastructure.DataAccess.Repositories
{
    internal class BaseRepository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _dbContext;

        internal BaseRepository(AppDbContext dbContext)
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
    }
}
