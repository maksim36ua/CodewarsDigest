using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw_itkpi.Models
{
    public class UserContext : DbContext
    {
        public DbSet<UserInfo> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Make NameId required
            modelBuilder.Entity<UserInfo>()
                .Property(b => b.username)
                .IsRequired();
        }
        
    }
}
