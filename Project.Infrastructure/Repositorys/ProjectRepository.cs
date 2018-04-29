using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Project.Domain.AggregatesModel;
using Project.Domain.SeedWork;
using PrjectEntity = Project.Domain.AggregatesModel.Project;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Project.Infrastructure.Repositorys
{
    public class ProjectRepository : IProjectRepository
    {

        private readonly ProjectContext _dbContext;
        public ProjectRepository(ProjectContext projectContext)
        {
            _dbContext = projectContext;
        }
        public IUnitOfWork UnitOfWork => _dbContext;
       

        public PrjectEntity Add(PrjectEntity project)
        {
            if(project.IsTransient())
            {
                return _dbContext.Add(project).Entity;
            }
            return project;
        }

        public async Task<PrjectEntity> GetAsync(int id)
        {
             var project =    await _dbContext.Projects
                        .Include(p => p.Properties)
                        .Include(p => p.Viewers)
                        .Include(a => a.Contributors)
                        .Include(a => a.VisibleRule)
                        .SingleOrDefaultAsync(b => b.Id == id);
            return project;
                      
        }

        public void Update(PrjectEntity project)
        {
            _dbContext.Projects.Update(project);        
        }
    }
}
