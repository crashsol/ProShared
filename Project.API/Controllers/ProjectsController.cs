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

namespace Project.API.Controllers
{
    [Route("api/[controller]")]
    public class ProjectsController : BaseController
    {
        private readonly IMediator _mediator;

        public ProjectsController(ILogger<ProjectsController> logger, IMediator mediator) : base(logger)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateAsync([FromBody]Project.Domain.AggregatesModel.Project project)
        {
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
        [Route("join")]
        public async Task<IActionResult> JoinAsync([FromBody]ProjectContributor projectContributor)
        {      
            var joinProjectCommand = new JoinProjectCommand { Contributor = projectContributor };         
            await _mediator.Send(joinProjectCommand);
            return Ok();          
        }
    }
}
