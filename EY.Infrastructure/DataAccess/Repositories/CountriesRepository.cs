using EY.Domain.Contracts;
using EY.Domain.Countries;
using EY.Shared.Attributes;

namespace EY.Infrastructure.DataAccess.Repositories;

[BindInterface(typeof(IRepository<Country>))]
[BindInterface(typeof(IRepositoryBulk<Country>))]
public class CountriesRepository : BaseRepository<Country>
{
    public CountriesRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}