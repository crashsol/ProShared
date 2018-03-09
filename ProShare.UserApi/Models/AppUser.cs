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

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }


        /// <summary>
        /// 头像地址 来着第三方存储
        /// </summary>
        public string  Avatar { get; set; }

        /// <summary>
        /// 性别 1：男  0：女
        /// </summary>
        public byte Gender { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string Email { get; set; }



        /// <summary>
        /// 省份ID
        /// </summary>
        public string ProvinceId { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }


        public string CityId { get; set; }

        public string City { get; set; }

        public string NameCard { get; set; }

        public List<UserProperty> Properties { get; set; }


    }

   
}
