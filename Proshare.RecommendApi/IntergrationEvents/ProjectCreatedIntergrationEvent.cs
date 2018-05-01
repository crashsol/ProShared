using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proshare.RecommendApi.IntergrationEvents
{
    public class ProjectCreatedIntergrationEvent
    {
        public int ProjectId { get; set; }

        public string ProjectAvatar { get; set; }


        public string Company { get; set; }


        /// <summary>
        /// 项目简介
        /// </summary>
        public string Introducation { get; set; }

        /// <summary>
        /// 项目标签
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 融资阶段
        /// </summary>
        public string FinStage { get; set; }
     


        public int UserId { get; set; }

        public DateTime CreatedTime { get; set; }

    }
}
