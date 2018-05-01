using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proshare.RecommendApi.IntergrationEvents
{
    public class ProjectCreatedIntergrationEvent
    {
        public int ProjectId { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedTime { get; set; }

    }
}
