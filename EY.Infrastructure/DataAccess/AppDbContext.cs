using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Infrastructure.DataAccess
{
    public class AppDbContext : DbContext
    {

        public AppDbContext()
        {            
        }
        public AppDbContext(DbContextOptions options) : base(options)
        {            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //TODO: Add maps inherited from EntityMapper
            base.OnModelCreating(modelBuilder);
        }
    }
}
