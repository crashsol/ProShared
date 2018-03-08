using Microsoft.EntityFrameworkCore;
using ProShare.UserApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProShare.UserApi.Data
{
    public class UserContext:DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>(b =>
            {
                b.ToTable("Users");
            });

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<AppUser> Users { get; set; }
    }
}
