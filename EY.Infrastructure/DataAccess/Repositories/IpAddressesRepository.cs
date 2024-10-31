using EY.Domain.Contracts;
using EY.Domain.IpAddresses;
using EY.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Infrastructure.DataAccess.Repositories
{
    [BindInterface(typeof(IRepository<IpAddress>))]
    [BindInterface(typeof(IRepositoryBulk<IpAddress>))]
    public class IpAddressesRepository : BaseRepository<IpAddress>
    {
        public IpAddressesRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
