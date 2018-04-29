using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.EntityConfigurations
{   

    public class ProjectPropertyEntityConfiguration : IEntityTypeConfiguration<Project.Domain.AggregatesModel.ProjectProperty>
    {
        public void Configure(EntityTypeBuilder<Domain.AggregatesModel.ProjectProperty> builder)
        {
            builder.ToTable("ProjectProperties")
                .HasKey( p=> new { p.ProjectId ,p.Key,p.Value });
            builder.Property(b => b.Key).HasMaxLength(100);
            builder.Property(b => b.Value).HasMaxLength(100);
        }
    }
}
