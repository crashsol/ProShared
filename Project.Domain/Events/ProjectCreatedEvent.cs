
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MediatR;

namespace Project.Domain.Events
{
    /// <summary>
    /// 项目创建事件
    /// </summary>
    public class ProjectCreatedEvent:INotification
    {
        public AggregatesModel.Project Project { get; set; }
    }
}