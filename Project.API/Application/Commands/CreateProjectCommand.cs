using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Project.API.Application.Commands
{
    /// <summary>
    /// 创建项目命令
    /// </summary>
    public class CreateProjectCommand: IRequest<Domain.AggregatesModel.Project>
    {
        public Domain.AggregatesModel.Project Project { get; set; }
    }
}
