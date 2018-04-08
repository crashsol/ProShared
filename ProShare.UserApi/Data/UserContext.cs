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

            //注意： Mysql中,如果如果用string类型作为主键，必须限定长度小于255

            //用户配置
            modelBuilder.Entity<AppUser>(b =>
            {
                b.ToTable("Users");
                b.HasKey(a => a.Id);
            });

            //用户属性配置
            modelBuilder.Entity<UserProperty>(b =>
            {
                b.ToTable("UserProperties");
                b.Property(a => a.Key).HasMaxLength(100);   //限定主键长度
                b.Property(a => a.Value).HasMaxLength(100); //限定主键长度              
                b.HasKey(a => new { a.AppUserId, a.Key, a.Value });//主键唯一
            });
            //用户标签
            modelBuilder.Entity<UserTag>(b =>
            {
                b.ToTable("UserTags");
                b.Property(a => a.Tag).HasMaxLength(100);  //限定主键长度
                b.HasKey(a => new { a.AppUserId, a.Tag });
            });

            modelBuilder.Entity<BPFile>(b => {
                b.ToTable("BPFiles");
                b.HasKey(a => a.Id);
            });

            base.OnModelCreating(modelBuilder);
        }


        /// <summary>
        /// 用户集合
        /// </summary>
        public DbSet<AppUser> Users { get; set; }

        /// <summary>
        /// 用户管理属性集合
        /// </summary>
        public DbSet<UserProperty> UserProperties { get; set; }

        /// <summary>
        /// 用户标签数据
        /// </summary>
        public DbSet<UserTag> UserTags { get; set; }
    }
}
