using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProShare.UserApi.Models
{
    /// <summary>
    /// 用户标签
    /// </summary>
    public class UserTag
    {

        public int AppUserId { get; set; }

        public string Tag { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
