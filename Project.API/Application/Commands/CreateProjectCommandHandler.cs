using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Project.Domain;
using Project.Domain.AggregatesModel;
using Project.Domain.SeedWork;
using Project.Domain.Exceptions;

namespace Project.API.Application.Commands
{
    /// <summary>
    /// 创建项目消息处理
    /// </summary>
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Project.Domain.AggregatesModel.Project>
    {
        public IProjectRepository _projectRepository;
        public CreateProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<Domain.AggregatesModel.Project> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            await _projectRepository.AddAsync(request.Project);
            await _projectRepository.UnitOfWork.SaveEntitiesAsync();         

            return request.Project;
        }
    }
}
