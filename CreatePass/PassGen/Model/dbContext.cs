using Microsoft.EntityFrameworkCore;
using CreatePass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatePass
{
    public class CreatePassContext : DbContext
    {
        public DbSet<SiteKeyItem> SiteKeys { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=CreatePass.db");
        }
    }
}
