using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Project.API.Application.Commands
{
    /// <summary>
    /// 项目查看事件
    /// </summary>
    public class ViewProjectCommand:IRequest
    {

        public int ProjectId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Avatar { get; set; }
    }
}
