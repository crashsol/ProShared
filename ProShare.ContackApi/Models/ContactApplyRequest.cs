﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProShare.ContactApi.Models
{
    /// <summary>
    /// 好友申请
    /// </summary>
    /// 忽略内中不存在的字段 _Id
    [BsonIgnoreExtraElements]
    public class ContactApplyRequest
    {
        /// <summary>
        /// 被申请用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 申请人名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 申请人公司
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 申请人职位
        /// </summary>
        public string Title { get; set; }


        /// <summary>
        /// 申请人头像
        /// </summary>
        public string Avatar { get; set; }


        /// <summary>
        /// 申请人ID
        /// </summary>
        public int ApplierId { get; set; }

        /// <summary>
        /// 是否通过 0未通过 1已通过
        /// </summary>
        public int Approvaled { get; set; }


        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime HandleTime { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyTime { get; set; }
    }
}
