using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Infrastructure.Contracts
{
    public interface IMigrationsExecuter
    {
        void Migrate();
    }
}
