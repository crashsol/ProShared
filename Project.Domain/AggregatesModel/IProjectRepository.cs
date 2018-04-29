using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Project.Domain.SeedWork;

namespace Project.Domain.AggregatesModel
{
    public interface IProjectRepository : IProjectRepository<Project>
    {
        Task<Project> GetAsync(int id);
        Project Add(Project project);
        void Update(Project project);
    }
}
