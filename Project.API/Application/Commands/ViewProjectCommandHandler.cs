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
    /// 项目查看事件处理
    /// </summary>
    public class ViewProjectCommandHandler : IRequestHandler<ViewProjectCommand>
    {

        public IProjectRepository _projectRepository;
        public ViewProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task Handle(ViewProjectCommand request, CancellationToken cancellationToken)
        {
           var project = await  _projectRepository.GetAsync(request.ProjectId);

           if(project == null) throw new ProjectDomainException($"项目不存在: {request.ProjectId}");

            project.AddViewer(request.UserId, request.UserName, request.Avatar);

           await _projectRepository.UnitOfWork.SaveEntitiesAsync();

        }
    }
}
