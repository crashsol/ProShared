using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.AggregatesModel;

namespace Project.Infrastructure.EntityConfigurations
{
    public class ProjectContributorEntityConfiguration : IEntityTypeConfiguration<Project.Domain.AggregatesModel.ProjectContributor>
    {
        public void Configure(EntityTypeBuilder<Domain.AggregatesModel.ProjectContributor> builder)
        {
            builder.ToTable("ProjectContributors").HasKey(p => p.Id);
        }
    }
}