using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace ProShare.ContactApi.Models
{
    /// <summary>
    /// 好友信息
    /// </summary>
    /// 
    [BsonIgnoreExtraElements]
    public class Contact
    {

        public Contact()
        {
            Tags = new List<string>();
        }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string  Name { get; set; }

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

        /// <summary>
        /// 用户标签
        /// </summary>
        public List<string> Tags { get; set; }

    }
}
