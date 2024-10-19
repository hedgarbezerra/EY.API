using EY.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Infrastructure.DataAccess
{
    public class UnityOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;
        private Dictionary<Type, IRepository<object>> _repositories;

        public UnityOfWork(AppDbContext appDbContext, IEnumerable<IRepository<object>> repositories)
        {
            _appDbContext = appDbContext;
            _repositories = repositories.ToDictionary(k => typeof(k)),;
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IRepository<T> Repository<T>() where T : class
        {
            throw new NotImplementedException();
        }
    }
}
