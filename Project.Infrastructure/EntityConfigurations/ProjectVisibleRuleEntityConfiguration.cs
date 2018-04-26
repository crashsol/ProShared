using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.EntityConfigurations
{
   
    public class ProjectVisibleRuleEntityConfiguration : IEntityTypeConfiguration<Project.Domain.AggregatesModel.ProjectVisibleRule>
    {
        public void Configure(EntityTypeBuilder<Domain.AggregatesModel.ProjectVisibleRule> builder)
        {
            builder.ToTable("ProjectVisibleRules").HasKey(p => p.Id);
        }
    }
}
