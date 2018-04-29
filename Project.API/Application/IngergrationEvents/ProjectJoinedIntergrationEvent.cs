using Project.Domain.AggregatesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;

namespace Project.API.Application.IngergrationEvents
{
    public class ProjectJoinedIntergrationEvent
    {

        public string Company { get; set; }

        public string Introduction { get; set; }


        public ProjectContributor ProjectContributor { get; set; }
       
    }
}
