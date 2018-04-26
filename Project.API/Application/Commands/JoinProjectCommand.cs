using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Project.Domain.AggregatesModel;

namespace Project.API.Application.Commands
{
    /// <summary>
    /// 加入项目事件
    /// </summary>
    public class JoinProjectCommand:IRequest
    {

        public ProjectContributor Contributor { get; set; }

    }
}
