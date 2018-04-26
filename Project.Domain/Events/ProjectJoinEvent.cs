
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MediatR;
using Project.Domain.AggregatesModel;

namespace Project.Domain.Events
{

    /// <summary>
    /// 有人参与项目事件
    /// </summary>
    public class ProjectJoinEvent:INotification    {
        public ProjectContributor ProjectContributor { get; set; }
    }
}