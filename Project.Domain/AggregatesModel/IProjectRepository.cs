using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.AggregatesModel
{
    public interface IProjectRepository
    {
        Task<Project> GetAsync(int id);
        Task<Project> AddAsync(Project project);
        Task<Project> UpdateAsync(Project project);      
    }
}
