using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediatR;
using Project.API.Application.Commands;
using Infrastructure.OperationException;
using Project.Domain.AggregatesModel;
using Project.API.Application.Services;
using Project.API.Application.Queries;

namespace Project.API.Controllers
{
    [Route("api/[controller]")]
    public class ProjectsController : BaseController
    {
        private readonly IMediator _mediator;

        private readonly IRecommendService _recommendService;

        private readonly IProjectQueries _projectQueries;

        public ProjectsController(ILogger<ProjectsController> logger,
            IMediator mediator,
            IRecommendService recommendService,
            IProjectQueries projectQueries
            ) : base(logger)
        {
            _mediator = mediator;
            _recommendService = recommendService;
            _projectQueries = projectQueries;
        }

        /// <summary>
        /// 获取自己创建的项目列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetProjectsAsync()
        {
            return Ok(await _projectQueries.GetProjectsByUserIdAsync(UserIdentity.UserId));
        }
        /// <summary>
        /// 获取自己的项目详细信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("my/{projectId}")]
        public async Task<IActionResult> GetProjectDetailAsync(int projectId)
        {
            var result = await _projectQueries.GetProjectDetailAsync(projectId);
            if (result.UserId == UserIdentity.UserId)
            {
                return Ok(result);
            }
            else
            {
                throw new UserOperationException("无权查看该项目");
            }
        }

        [Route("recommends/{projectId}")]
        [HttpGet]
        public async Task<IActionResult> GetRecommendProjectDetailAsync(int projectId)
        {
            if (!(await _recommendService.IsProjectInRecommend(UserIdentity.UserId, projectId)))
            {
                throw new UserOperationException("无权查看该项目");
            }
            var result = await _projectQueries.GetProjectDetailAsync(projectId);
            return Ok(result);

        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateAsync([FromBody]Project.Domain.AggregatesModel.Project project)
        {
            if(project == null)
                throw new UserOperationException("参数为空");
            project.UserId = UserIdentity.UserId;
            var command = new CreateProjectCommand() { Project = project };
            var result = await _mediator.Send(command);

            if (result == null)
                throw new UserOperationException("创建项目失败");

            return Ok(result);
        }

        [HttpPut]
        [Route("view/{projectId}")]
        public async Task<IActionResult> ViewAsync(int projectId)
        {

            if (!(await _recommendService.IsProjectInRecommend(UserIdentity.UserId, projectId)))
            {
                throw new UserOperationException("无权查看该项目");
            }
            var viewCommand = new ViewProjectCommand()
            {
                Avatar = UserIdentity.Avatar,
                UserId = UserIdentity.UserId,
                ProjectId = projectId,
                UserName = UserIdentity.Name
            };
            await _mediator.Send(viewCommand);
            return Ok();
        }

        [HttpPut]
        [Route("join/{projectId}")]
        public async Task<IActionResult> JoinAsync(int projectId, [FromBody]ProjectContributor projectContributor)
        {
            if (!(await _recommendService.IsProjectInRecommend(UserIdentity.UserId, projectId)))
            {
                throw new UserOperationException("无权查看该项目");
            }
            var joinProjectCommand = new JoinProjectCommand { Contributor = projectContributor };
            await _mediator.Send(joinProjectCommand);
            return Ok();
        }
    }
}
