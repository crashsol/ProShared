using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProShare.UserApi.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        /// <summary>
        /// 姓名
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
    }
}
