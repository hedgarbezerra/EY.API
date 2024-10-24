using EY.Domain.Contracts;
using EY.Domain.Entities;
using EY.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Infrastructure.DataAccess.Repositories
{
    [BindInterface(typeof(IRepository<Country>))]
    [BindInterface(typeof(IRepositoryBulk<Country>))]
    public class CountriesRepository : BaseRepository<Country>
    {
        public CountriesRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
