using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Domain.Contracts
{
    public interface ISqlExecutor<T> where T : class
    {
        IQueryable<T> Query(FormattableString query);
        IQueryable<T> Query(string sql, params object[] parameters);
        int Execute(FormattableString query);
    }
}
