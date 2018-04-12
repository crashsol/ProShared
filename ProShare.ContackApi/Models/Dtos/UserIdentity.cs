using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProShare.ContactApi.Models.Dtos
{

    /// <summary>
    /// 登录用户信息
    /// </summary>
    public class UserIdentity
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string Title { get; set; }


        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }
    }
}
