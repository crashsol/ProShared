using DotNetCore.CAP;
using Project.Domain.AggregatesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.API.Application.IngergrationEvents
{
    public class ProjectViewedIntergrationEvent
    {
        public string Company { get; set; }

        public string Introduction { get; set; }

        public ProjectViewer Viewer { get; set; }
    }
}
