using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Domain.AggregatesModel
{

    /// <summary>
    /// 项目属性
    /// </summary>
    public class ProjectProperty:Entity
    {
        /// <summary>
        /// 关联用户ID
        /// </summary>
        public int ProjectId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public string Text { get; set; }
    }
}
