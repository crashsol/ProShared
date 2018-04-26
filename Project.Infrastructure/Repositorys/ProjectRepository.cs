using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Project.Domain.AggregatesModel;
using Project.Domain.SeedWork;
using PrjectEntity = Project.Domain.AggregatesModel.Project;

namespace Project.Infrastructure.Repositorys
{
    public class ProjectRepository : IProjectRepository
    {
        public IUnitOfWork UnitOfWork => throw new NotImplementedException();

        public Task<PrjectEntity> AddAsync(PrjectEntity project)
        {
            throw new NotImplementedException();
        }

        public Task<PrjectEntity> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PrjectEntity> UpdateAsync(PrjectEntity project)
        {
            throw new NotImplementedException();
        }
    }
}
