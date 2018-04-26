using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Project.Domain.AggregatesModel;
using Project.Domain.Exceptions;

namespace Project.API.Application.Commands
{

    /// <summary>
    /// 加入项目事件处理
    /// </summary>
    public class JoinProjectCommandHandler:IRequestHandler<JoinProjectCommand>
    {

        public IProjectRepository _projectRepository;
        public JoinProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task Handle(JoinProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetAsync(request.Contributor.ProjectId);
            if (project == null) throw new ProjectDomainException($"项目不存在: {request.Contributor.ProjectId}");

            project.AddContributor(request.Contributor);
             await _projectRepository.UnitOfWork.SaveEntitiesAsync();
        

        }
    }
}
