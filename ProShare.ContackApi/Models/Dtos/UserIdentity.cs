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

        public int UserId { get; set; }

        public string Name { get; set; }
    }
}
