using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Proshare.RecommendApi.Models;

namespace Proshare.RecommendApi.Data
{
    public class RecommendDbContext:DbContext
    {
        public RecommendDbContext(DbContextOptions<RecommendDbContext> dbContextOptions) : base(dbContextOptions) { }


        public DbSet<ProjectRecommend> ProjectRecommends { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectRecommend>()
                .ToTable("ProjectRecommend")
                .HasKey(b => b.Id);                    
        }
    }
}
